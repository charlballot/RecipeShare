using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Validation
{
    public class ValidIngredientsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not List<string> ingredients)
                return false;

            if (!ingredients.Any())
                return false;

            return ingredients.All(ingredient =>
                !string.IsNullOrWhiteSpace(ingredient) &&
                ingredient.Trim().Length >= 2);
        }

        public override string FormatErrorMessage(string name)
        {
            return "All ingredients must be at least 2 characters long and not empty.";
        }
    }
}
