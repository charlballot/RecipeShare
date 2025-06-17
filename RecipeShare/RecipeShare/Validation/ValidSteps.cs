using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Validation
{
    public class ValidStepsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not List<string> steps)
                return false;

            if (!steps.Any())
                return false;

            return steps.All(step =>
                !string.IsNullOrWhiteSpace(step) &&
                step.Trim().Length >= 10);
        }

        public override string FormatErrorMessage(string name)
        {
            return "All steps must be at least 10 characters long and not empty.";
        }
    }
}
