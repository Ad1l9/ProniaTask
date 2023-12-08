using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.ViewModel;
using System.Security.Claims;

namespace ProniaTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public BasketController(AppDbContext context,UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();

            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _usermanager.Users
                    .Include(u => u.BasketItems)
                    .ThenInclude(u => u.Product)
                    .ThenInclude(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new()
                    {
                        Name=item.Product.Name,
                        Price=item.Product.Price,
                        ImageUrl=item.Product.ProductImages.FirstOrDefault().ImageURL,
                        Count=item.Count,
                        SubTotal=item.Count*item.Product.Price
                    });
                }
            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    foreach (BasketCookieItemVM basketCookieItem in basket)
                    {
                        Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                        if (product is not null)
                        {
                            BasketItemVM basketItem = new()
                            {
                                Id = product.Id,
                                Name = product.Name,
                                ImageUrl = product.ProductImages.FirstOrDefault().ImageURL,
                                Price = product.Price,
                                Count = basketCookieItem.Count,
                                SubTotal = product.Price * basketCookieItem.Count,
                            };
                            basketVM.Add(basketItem);
                        }

                    }

                }
            }

            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id, string controllername)
        {
            if (id <= 0) return BadRequest();
            
            Product product= await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);

            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _usermanager.Users.Include(u => u.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

                BasketItem basketItem=user.BasketItems.FirstOrDefault(bi => bi.Id == id);

                if(basketItem is null)
                {
                    basketItem = new()
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1
                    };
                    user.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                
               
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);
                    if (item is null)
                    {
                        BasketCookieItemVM itemVM = new()
                        {
                            Id = product.Id,
                            Count = 1
                        };
                        basket.Add(itemVM);
                    }
                    else
                    {
                        item.Count++;
                    }
                }
                else
                {
                    basket = new();
                    BasketCookieItemVM itemVM = new()
                    {
                        Id = product.Id,
                        Count = 1
                    };
                    basket.Add(itemVM);
                }



                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);
            }

            

            return RedirectToAction(nameof(Index),controllername);
            
        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            List<BasketCookieItemVM> basket;

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users.Include(u => u.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (user is null) return NotFound();
                var basketItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (basketItem is null) return NotFound();
                else
                {
                    user.BasketItems.Remove(basketItem);
                }
                await _context.SaveChangesAsync();
            }

            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);
                    if (item is not null)
                    {
                        basket.Remove(item);

                        string json = JsonConvert.SerializeObject(basket);

                        Response.Cookies.Append("Basket", json);
                    }
                }
            }

                


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Decrement(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;
            if (Request.Cookies["Basket"] is not null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                var item = basket.FirstOrDefault(b => b.Id == id);
                if (item is not null)
                {
                    item.Count--;
                    if (item.Count == 0)
                    {
                        basket.Remove(item);
                    }
                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("Basket", json);
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
