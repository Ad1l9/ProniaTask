﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Pronia.Utilities.Extentions;
using ProniaTask.Areas.ProniaAdmin.ViewModels;
using ProniaTask.DAL;
using ProniaTask.Enumerations;
using ProniaTask.Models;

namespace ProniaTask.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Products.CountAsync();
            List<Product> Products = await _context.Products.Skip(page * 3).Take(3)
                .Include(p => p.Category)
                .Include(p => p.ProductImages
                .Where(pi => pi.IsPrimary == true))
                .ToListAsync();

            PaginationVM<Product> pagination = new()
            {
                TotalPage = Math.Ceiling(count / 3),
                CurrentPage = page,
                Items = Products
            };
            return View(pagination);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                return View(productVM);
            }

            bool result = await _context.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);

            if (!result)
            {
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId","Bu id-li category movcud deyil");
                return View(productVM);
            }



            
            foreach (int id in productVM.TagIds)
            {
                bool TagResult = await _context.Tags.AnyAsync(t => t.Id == id);
                if (!TagResult)
                {
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bele tag movcud deyil");
                    return View(productVM);
                }
            }
            foreach (int id in productVM.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == id);
                if (!colorResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "Bele color movcud deyil");
                    return View(productVM);
                }
            }
            foreach (int id in productVM.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == id);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "Bele size movcud deyil");
                    return View(productVM);
                }
            }


            
            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File tipi uygun deyil");
                return View();
            }
            if (!productVM.MainPhoto.ValidateSize(600))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil");
                return View();
            }


            if (!productVM.HoverPhoto.ValidateType("image/"))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "File tipi uygun deyil");
                return View();
            }
            if (!productVM.HoverPhoto.ValidateSize(600))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "File olcusu uygun deyil");
                return View();
            }


            ProductImage mainImage = new()
            {
                Alternative=productVM.Name,
                IsPrimary = true,
                ImageURL =await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images"),
            };
            ProductImage hoverImage = new()
            {
                Alternative = productVM.Name,
                IsPrimary = false,
                ImageURL = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images"),
            };




            Product product = new()
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = (int)productVM.CategoryId,
                Description = productVM.Description,
                ProductTags = new(),
                ProductColors=new(),
                ProductSizes=new(),
                ProductImages = new List<ProductImage> { mainImage,hoverImage},
            };
            foreach (int id in productVM.TagIds)
            {
                var pTag = new ProductTag
                {
                    TagId = id,
                    Product = product
                };
                product.ProductTags.Add(pTag);
            }
            foreach (int id in productVM.ColorIds)
            {
                var pColor = new ProductColor
                {
                    ColorId = id,
                    Product = product
                };
                product.ProductColors.Add(pColor);
            }
            foreach (int id in productVM.SizeIds)
            {
                var pSize = new ProductSize
                {
                    SizeId = id,
                    Product = product
                };
                product.ProductSizes.Add(pSize);
            }


            TempData["Message"] = "";
            foreach (IFormFile image in productVM.Photos)
            {
                if (!image.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{image.FileName} file tipi uygun deyil</p>";
                    continue;
                }
                if (!image.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{image.FileName} file olcusu uygun deyil</p>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    ImageURL = await image.CreateFile(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = null,
                    Alternative = productVM.Name
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(pi => pi.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			var existed = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			foreach (ProductImage image in existed.ProductImages)
			{
				image.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
			}
			_context.Products.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            UpdateProductVM productVM = new()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                ProductImages=product.ProductImages
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
			Product existed = await _context.Products
				.Include(p => p.ProductTags)
				.Include(p => p.ProductColors)
				.Include(p => p.ProductSizes)
                .Include(p=>p.ProductImages)
				.FirstOrDefaultAsync(p => p.Id == id);

            productVM.ProductImages = existed.ProductImages;

			if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }
            
            if (existed is null) return NotFound();


            bool result = await _context.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }



			



			foreach (int idT in productVM.TagIds)
            {
                bool TagResult = await _context.Tags.AnyAsync(t => t.Id == idT);
                if (!TagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("TagIds", "bele tag yoxdur");
                    return View(productVM);
                }
            }
            foreach (int idC in productVM.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == idC);
                if (!colorResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("ColorIds", "Bele reng movcud deyil");
                    return View(productVM);
                }
            }
            foreach (int idS in productVM.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == idS);
                if (!sizeResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("SizeIds", "Bele size yoxdur");
                    return View(productVM);
                }
            }




			if (productVM.MainPhoto is not null)
			{
				if (!productVM.MainPhoto.ValidateType())
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File type is not valid");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File size is not valid");
					return View(productVM);
				}
			}
			if (productVM.HoverPhoto is not null)
			{
				if (!productVM.HoverPhoto.ValidateType())
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("HoverPhoto", "File type is not valid");
					return View(productVM);
				}
				if (!productVM.HoverPhoto.ValidateSize(600))
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					ModelState.AddModelError("HoverPhoto", "File size is not valid");
					return View(productVM);
				}
			}

			if (productVM.MainPhoto is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
				mainImage.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(mainImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative = productVM.Name,
					IsPrimary = true,
					ImageURL = fileName
				});
			}
			if (productVM.HoverPhoto is not null)
			{
				string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
				ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
				hoverImage.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				_context.ProductImages.Remove(hoverImage);
				existed.ProductImages.Add(new ProductImage
				{
					Alternative = productVM.Name,
					IsPrimary = false,
					ImageURL = fileName
				});
			}
			if (productVM.ImageIds is null)
			{
				productVM.ImageIds = new();
			}
			var removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
			foreach (ProductImage pi in removeable)
			{
				pi.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
				existed.ProductImages.Remove(pi);
			}




			result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("Name", "Bele product movcuddur");
                return View(productVM);
            }

            existed.ProductTags.RemoveAll(pTag => !productVM.TagIds.Contains(pTag.Id));

            existed.ProductTags.AddRange(productVM.TagIds.Where(tagId => !existed.ProductTags.Any(pt => pt.Id == tagId))
                                 .Select(tagId => new ProductTag { TagId = tagId }));

            existed.ProductColors.RemoveAll(pColor => !productVM.ColorIds.Contains(pColor.Id));

            existed.ProductColors.AddRange(productVM.ColorIds.Where(colorId => !existed.ProductColors.Any(pt => pt.Id == colorId))
                                 .Select(colorId => new ProductColor { ColorId = colorId }));

            existed.ProductSizes.RemoveAll(pSize => !productVM.SizeIds.Contains(pSize.Id));

            existed.ProductSizes.AddRange(productVM.SizeIds.Where(sizeId => !existed.ProductSizes.Any(pt => pt.Id == sizeId))
                                 .Select(sizeId => new ProductSize { SizeId = sizeId }));


			TempData["Message"] = "";
			if (productVM.Photos is not null)
			{
				foreach (IFormFile photo in productVM.Photos)
				{
					if (!photo.ValidateType())
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
						continue;
					}
					if (!photo.ValidateSize(600))
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
						continue;
					}

					existed.ProductImages.Add(new ProductImage
					{
						Alternative = productVM.Name,
						IsPrimary = null,
						ImageURL = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
					});
				}
			}


			existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
