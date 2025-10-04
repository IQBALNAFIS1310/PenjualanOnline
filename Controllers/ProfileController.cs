using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenjualanOnline.Data;
using PenjualanOnline.Models;

namespace PenjualanOnline.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Auth");

            var guid = Guid.Parse(userId);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == guid);

            if (account == null) return NotFound();

            return View(account);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Auth");

            var guid = Guid.Parse(userId);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == guid);

            if (account == null) return NotFound();

            return View(account);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Auth");

            var guid = Guid.Parse(userId);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == guid);

            if (account == null) return NotFound();

            try
            {
                // update hanya field yg boleh diedit
                account.Name = model.Name;
                account.Address = model.Address;
                account.Phone = model.Phone;

                _context.Update(account);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profil berhasil diperbarui.";
            }
            catch
            {
                TempData["Error"] = "Gagal memperbarui profil.";
            }

            return RedirectToAction("Index");
        }

        // POST: /Profile/BecomeSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeSeller()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Auth");

            var guid = Guid.Parse(userId);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == guid);

            if (account == null) return NotFound();

            account.Role = "pending";
            _context.Update(account);
            await _context.SaveChangesAsync();

            // Refresh claims biar role langsung berubah
            await RefreshUserClaims(account);

            TempData["Success"] = "Selamat, Anda Berhasil mengajukan diri menjadi penjual, MOHON TUNGGU KONFIRMASI!";
            return RedirectToAction("Index");
        }

        private async Task RefreshUserClaims(Account account)
        {
            // Hapus claim lama
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Buat claim baru dengan role yang diupdate
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.Role, account.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
    }
}
