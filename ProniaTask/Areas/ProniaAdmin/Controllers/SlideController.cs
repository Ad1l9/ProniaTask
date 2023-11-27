using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Utilities.Extentions;
using ProniaTask.Areas.ProniaAdmin.ViewModels;
using ProniaTask.DAL;

using ProniaTask.Models;

namespace ProniaTask.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var slides = await _context.Slides.ToListAsync();
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slideVM.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Yanlis fayl tipi");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "2mb dan cox olmamalidir");
                return View();
            }

            string fileName= await slideVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");


            Slide slide = new()
            {
                ImageUrl=fileName,
                Title=slideVM.Title,
                Subtitle=slideVM.Subtitle,
                Description=slideVM.Description,
                Order=slideVM.Order,

            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            return View(slide);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            if (slide is null) return NotFound();

            UpdateSlideVM updateSlideVM = new()
            {
                ImageUrl = slide.ImageUrl,
                Title = slide.Title,
                Subtitle = slide.Subtitle,
                Order = slide.Order,
                Description=slide.Description
            };

            return View(updateSlideVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM updateSlideVM)
        {
            if (!ModelState.IsValid) return View(updateSlideVM);


            Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            
            
            bool result = _context.Slides.Any(c => c.Title == updateSlideVM.Title && c.Order == updateSlideVM.Order && c.Id != id);
            if (!result)
            {
                if (updateSlideVM.Photo is not null)
                {
                    if (!updateSlideVM.Photo.ValidateType())
                    {
                        ModelState.AddModelError("Photo", "Yanlis fayl tipi");
                        return View(updateSlideVM);
                    }
                    if (!updateSlideVM.Photo.ValidateSize(2 * 1024))
                    {
                        ModelState.AddModelError("Photo", "2mb dan cox olmamalidir");
                        return View(updateSlideVM);
                    }
                    string newImage = await updateSlideVM.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
                    existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                    existed.ImageUrl = newImage;

                }

                existed.Title = updateSlideVM.Title;
                existed.Description = updateSlideVM.Description;
                existed.Subtitle = updateSlideVM.Subtitle;
                existed.Order = updateSlideVM.Order;
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("Title", "Bu title istifade olunub");
                ModelState.AddModelError("Order", "Bu order istifade olunub");
                return View(existed);
            }


            return RedirectToAction(nameof(Index));
        }
    }
}