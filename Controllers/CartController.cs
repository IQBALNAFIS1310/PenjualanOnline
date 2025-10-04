using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenjualanOnline.Data;
using PenjualanOnline.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PenjualanOnline.Controllers
{
    [Authorize(Roles = "user,seller,pending")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Auth");

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.Carts.Add(newCart);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Produk ditambahkan ke keranjang.";
            return RedirectToAction("Index");
        }

        // POST: /Cart/Update
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Keranjang diperbarui.";
            }
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produk dihapus dari keranjang.";
            }
            return RedirectToAction("Index");
        }
    }
}
