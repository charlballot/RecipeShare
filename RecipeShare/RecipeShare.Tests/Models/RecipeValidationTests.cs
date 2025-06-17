using FluentAssertions;
using RecipeShare.DTOs;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Tests.Models
{
    public class RecipeValidationTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CreateRecipeDto_IsValid_WhenAllFieldsAreCorrect()
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = "Valid Recipe Title",
                Ingredients = new List<string> { "Ingredient 1", "Ingredient 2" },
                Steps = new List<string> { "Step 1: Prepare ingredients", "Step 2: Cook everything" },
                CookingTime = 30,
                DietaryTags = new List<string> { "Vegan", "Quick" }
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("AB")] // Too short
        public void CreateRecipeDto_IsInvalid_WhenTitleIsInvalid(string title)
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = title,
                Ingredients = new List<string> { "Valid Ingredient" },
                Steps = new List<string> { "Valid step with enough characters" },
                CookingTime = 30
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Title"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1441)] // Too long (more than 24 hours)
        public void CreateRecipeDto_IsInvalid_WhenCookingTimeIsInvalid(int cookingTime)
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = "Valid Title",
                Ingredients = new List<string> { "Valid Ingredient" },
                Steps = new List<string> { "Valid step with enough characters" },
                CookingTime = cookingTime
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("CookingTime"));
        }

        [Fact]
        public void CreateRecipeDto_IsInvalid_WhenIngredientsAreEmpty()
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = "Valid Title",
                Ingredients = new List<string>(),
                Steps = new List<string> { "Valid step with enough characters" },
                CookingTime = 30
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Ingredients"));
        }

        [Fact]
        public void CreateRecipeDto_IsInvalid_WhenStepsAreTooShort()
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = "Valid Title",
                Ingredients = new List<string> { "Valid Ingredient" },
                Steps = new List<string> { "Short" }, // Too short
                CookingTime = 30
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Steps"));
        }

        [Fact]
        public void CreateRecipeDto_IsInvalid_WhenDietaryTagsAreInvalid()
        {
            // Arrange
            var recipe = new CreateRecipeDto
            {
                Title = "Valid Title",
                Ingredients = new List<string> { "Valid Ingredient" },
                Steps = new List<string> { "Valid step with enough characters" },
                CookingTime = 30,
                DietaryTags = new List<string> { "InvalidTag" }
            };

            // Act
            var validationResults = ValidateModel(recipe);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("DietaryTags"));
        }
    }
}
