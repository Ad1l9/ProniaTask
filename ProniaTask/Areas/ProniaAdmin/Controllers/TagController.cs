﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.Areas.ProniaAdmin.ViewModels;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();
            return View(Tags);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Tags.Any(c => c.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View();
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (existed is null) return NotFound();

            _context.Tags.Remove(existed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null) return NotFound();

            return View(tag);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool isInclude = _context.Tags.Any(c => c.Name == tagVM.Name && c.Id != id);

            if (isInclude)
            {
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();
            }

            existed.Name = tagVM.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}