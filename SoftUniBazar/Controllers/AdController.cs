using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
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

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}
	}
}
