using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Common.EntityValidationConstants.CategoryConstants;

namespace SoftUniBazar.Data.Models
{
	public class Category
	{
        public Category()
        {
            this.Ads = new HashSet<Ad>();
        }

        [Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(NameMaxLength)]
		public string Name { get; set; } = null!;

        public virtual ICollection<Ad> Ads { get; set; }
    }
}