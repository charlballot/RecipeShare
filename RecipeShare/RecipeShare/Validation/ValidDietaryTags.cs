using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Validation
{
    public class ValidDietaryTagsAttribute : ValidationAttribute
    {
        private readonly string[] _validTags =
        {
            "Vegan", "Vegetarian", "Gluten-Free", "Dairy-Free", "Keto",
            "Paleo", "Low-Carb", "High-Protein", "Quick", "Comfort Food",
            "Italian", "Asian", "Mexican", "Mediterranean", "Healthy"
        };

        public override bool IsValid(object? value)
        {
            if (value is not List<string> tags)
                return true; // Optional field

            return tags.All(tag => _validTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Invalid dietary tags. Valid options are: {string.Join(", ", _validTags)}";
        }
    }
}
