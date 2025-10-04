using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PenjualanOnline.Data;
using PenjualanOnline.Models;

namespace PenjualanOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                // kalau admin masuk ke dashboard admin
                return RedirectToAction("Index", "Admin");
            }
            var products = _context.Products.ToList();
            return View(products);
        }

        // hanya user login yang bisa lihat detail
        [Authorize]
        public IActionResult Detail(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}