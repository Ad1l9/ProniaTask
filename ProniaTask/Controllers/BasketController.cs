using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.ViewModel;

namespace ProniaTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();
            if(Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                foreach (BasketCookieItemVM basketCookieItem in basket)
                {
                    Product product = await _context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=>p.Id==basketCookieItem.Id);
                    if(product is not null)
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
            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id, string controllername)
        {
            if (id <= 0) return BadRequest();
            
            Product product= await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);

            if (product is null) return NotFound();

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

            return RedirectToAction(nameof(Index),controllername);
            
        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            List<BasketCookieItemVM> basket;

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
