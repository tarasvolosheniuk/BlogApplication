using BlogApplication.Data;
using BlogApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    public class HomeController:Controller
    {
        private AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Post()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Edit()
        {
            return View(new Post());
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
