using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SoftUniBazar.Common.EntityValidationConstants.AdConstants;

namespace SoftUniBazar.Data.Models
{
	public class Ad
	{
        public Ad()
        {
            CreatedOn = DateTime.UtcNow;
        }

        [Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(NameMaxLength)]
		public string Name { get; set; } = null!;

		[Required]
		[MaxLength(DescriptionMaxLength)]
		public string Description { get; set; } = null!;

		[Required]
		public decimal Price { get; set; } 

		[Required]
		public string OwnerId { get; set; } = null!;

		[Required]
		public IdentityUser Owner { get; set; } = null!;

		[Required]
		public string ImageUrl { get; set; } = null!;

		[Required]
		[DisplayFormat(DataFormatString = "yyyy-MM-dd H:mm")]
		public DateTime CreatedOn { get; set; }

		[Required]
		public int CategoryId { get; set; }

		[Required]
		[ForeignKey(nameof(CategoryId))]
		public Category Category { get; set; } = null!;
	}
}
