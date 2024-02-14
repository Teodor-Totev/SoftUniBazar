using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models;
using System.Security.Claims;
using System.Xml.Linq;

namespace SoftUniBazar.Controllers
{
    public class AdController : Controller
    {
        private readonly BazarDbContext context;

        public AdController(BazarDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> All()
        {
            IEnumerable<AdViewModel> ads = await context.Ads
                .Select(a => new AdViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Owner = a.Owner.UserName,
                    CreatedOn = a.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                    Category = a.Category.Name,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                })
                .ToArrayAsync();

            return View(ads);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            IEnumerable<CategoryViewModel> categories = await GetCategories();

            AdFormVM model = new()
            {
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Ad ad = new()
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                CategoryId = model.CategoryId,
                OwnerId = GetUserId()
            };

            await context.Ads.AddAsync(ad);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await context.Ads
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (ad == null)
            {
                return BadRequest();
            }

            IEnumerable<CategoryViewModel> categories = await GetCategories();

            AdFormVM model = new AdFormVM()
            {
                Name = ad.Name,
                Description = ad.Description,
                ImageUrl = ad.ImageUrl,
                Price = ad.Price,
                CategoryId = ad.CategoryId,
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ad = await context.Ads
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (ad == null)
            {
                return BadRequest();
            }

            ad.Name = model.Name;
            ad.Description = model.Description;
            ad.ImageUrl = model.ImageUrl;
            ad.Price = model.Price;
            ad.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var ad = await context.Ads
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            var currentUser = await context.Users
                .Where(u => u.Id == GetUserId())
                .FirstOrDefaultAsync();

            if (ad == null || currentUser == null)
            {
                return BadRequest();
            }

            if (context.AdsBuyers.Any(ab => ab.AdId == ad.Id))
            {
                return RedirectToAction("All");
            }

            await context.AdsBuyers.AddAsync(new AdBuyer()
            {
                AdId = ad.Id,
                BuyerId = currentUser.Id,
            });

            await context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            string userId = GetUserId();
            AdBuyer adBuyer = await context.AdsBuyers
                .Where(ab => ab.BuyerId == userId && ab.AdId == id)
                .FirstOrDefaultAsync();

            if (adBuyer == null)
            {
                return BadRequest();
            }

            context.AdsBuyers.Remove(adBuyer);
            await context.SaveChangesAsync();

            return RedirectToAction("All");
        }


        public async Task<IActionResult> Cart()
        {
            IEnumerable<AdViewModel> model = await context.AdsBuyers
                .Where(ab => ab.BuyerId == GetUserId())
                .Select(a => new AdViewModel()
                {
                    Id = a.Ad.Id,
                    Name = a.Ad.Name,
                    ImageUrl = a.Ad.ImageUrl,
                    CreatedOn = a.Ad.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                    Category = a.Ad.Category.Name,
                    Description = a.Ad.Description,
                    Price = a.Ad.Price,
                    Owner = a.Ad.Owner.UserName
                })
                .ToArrayAsync();

            return View(model);
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            IEnumerable<CategoryViewModel> categories = await context.Categories
                .Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToArrayAsync();

            return categories;
        }
    }
}
