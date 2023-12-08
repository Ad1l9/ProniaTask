using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.ViewModel;
using System.Security.Claims;

namespace ProniaTask.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {


        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<AppUser> _usermanager;

        public HeaderViewComponent(AppDbContext context,IHttpContextAccessor accessor,UserManager<AppUser> usermanager)
        {
            _context = context;
            _accessor = accessor;
            _usermanager = usermanager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<BasketItemVM> basketVM = new();

            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                if (_accessor.HttpContext.Request.Cookies["Basket"] != null)
                {
                    _accessor.HttpContext.Response.Cookies.Equals(null);
                }
                AppUser user = await _usermanager.Users
                    .Include(u => u.BasketItems)
                    .ThenInclude(u => u.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new()
                    {
                        Id=item.Id,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        ImageUrl = item.Product.ProductImages.FirstOrDefault().ImageURL,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price
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

            
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            HeaderVM vm = new() { Settings = settings, Basket = basketVM };

            return View(vm);
        }

    }
}
