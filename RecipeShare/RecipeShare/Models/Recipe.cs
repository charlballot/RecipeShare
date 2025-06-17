using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public List<string> Ingredients { get; set; } = new List<string>();

        [Required]
        public List<string> Steps { get; set; } = new List<string>();

        [Range(1, int.MaxValue, ErrorMessage = "Cooking time must be at least 1 munute")]
        public int CookingTime { get; set; }

        public List<string> DietaryTags { get; set; } = new List<string>();

        public DateTime CreatedAt {  get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
    }
}
