using System.ComponentModel.DataAnnotations;
using RecipeShare.Validation;

namespace RecipeShare.DTOs
{
    public class CreateRecipeDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one ingredient is required")]
        [ValidIngredients]
        public List<string> Ingredients { get; set; } = new List<string>();

        [Required(ErrorMessage = "At least one step is required")]
        [ValidSteps]
        public List<string> Steps { get; set; } = new List<string>();

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes (24 hours)")]
        public int CookingTime { get; set; }

        [ValidDietaryTags]
        public List<string> DietaryTags { get; set; } = new List<string>();
    }
}
