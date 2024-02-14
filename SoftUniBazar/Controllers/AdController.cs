using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models;
using System.Security.Claims;

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
			IEnumerable<CategoryViewModel> categories = await context.Categories
				.Select(c => new CategoryViewModel()
				{
					Id = c.Id,
					Name = c.Name,
				})
				.ToArrayAsync();

			CreateAdVM model = new()
			{
				Categories = categories
			};

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateAdVM a)
        {


			Ad ad = new()
			{
				Name = a.Name,
				Description = a.Description,
				ImageUrl = a.ImageUrl,
				Price = a.Price,
				CategoryId = a.CategoryId,
				OwnerId = GetUserId()
            };

			await context.Ads.AddAsync(ad);
			await context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}
	}
}
