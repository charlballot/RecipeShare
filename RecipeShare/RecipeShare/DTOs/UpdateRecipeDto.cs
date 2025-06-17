using RecipeShare.Validation;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.DTOs
{
    public class UpdateRecipeDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string? Title { get; set; }

        [ValidIngredients]
        public List<string>? Ingredients { get; set; }

        [ValidSteps]
        public List<string>? Steps { get; set; }

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes (24 hours)")]
        public int? CookingTime { get; set; }

        [ValidDietaryTags]
        public List<string>? DietaryTags { get; set; }
    }
}
