using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Controllers;
using RecipeShare.Data;
using RecipeShare.DTOs;
using RecipeShare.Models;
using RecipeShare.Profiles;

namespace RecipeShare.Tests.Controllers
{
    public class RecipesControllerTests : IDisposable
    {
        private readonly RecipeContext _context;
        private readonly RecipesController _controller;
        private readonly IMapper _mapper;

        public RecipesControllerTests()
        {
            var options = new DbContextOptionsBuilder<RecipeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecipeContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _controller = new RecipesController(_context, _mapper);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Test Recipe 1",
                    Ingredients = new List<string> { "Ingredient 1", "Ingredient 2" },
                    Steps = new List<string> { "Step 1: Do something", "Step 2: Do something else" },
                    CookingTime = 30,
                    DietaryTags = new List<string> { "Vegan", "Quick" }
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Test Recipe 2",
                    Ingredients = new List<string> { "Ingredient 3", "Ingredient 4" },
                    Steps = new List<string> { "Step 1: Prepare ingredients", "Step 2: Cook everything" },
                    CookingTime = 60,
                    DietaryTags = new List<string> { "Vegetarian" }
                }
            };

            _context.Recipes.AddRange(recipes);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetRecipes_ReturnsAllRecipes_WhenNoFiltersApplied()
        {
            // Act
            var result = await _controller.GetRecipes();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var recipes = okResult.Value.Should().BeAssignableTo<IEnumerable<RecipeDto>>().Subject;
            recipes.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetRecipes_FiltersCorrectly_WhenDietaryTagProvided()
        {
            // Act
            var result = await _controller.GetRecipes(dietaryTag: "Vegan");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var recipes = okResult.Value.Should().BeAssignableTo<IEnumerable<RecipeDto>>().Subject;
            recipes.Should().HaveCount(1);
            recipes.First().Title.Should().Be("Test Recipe 1");
        }

        [Fact]
        public async Task GetRecipes_FiltersCorrectly_WhenMaxCookingTimeProvided()
        {
            // Act
            var result = await _controller.GetRecipes(maxCookingTime: 45);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var recipes = okResult.Value.Should().BeAssignableTo<IEnumerable<RecipeDto>>().Subject;
            recipes.Should().HaveCount(1);
            recipes.First().CookingTime.Should().Be(30);
        }

        [Fact]
        public async Task GetRecipe_ReturnsRecipe_WhenValidIdProvided()
        {
            // Act
            var result = await _controller.GetRecipe(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var recipe = okResult.Value.Should().BeOfType<RecipeDto>().Subject;
            recipe.Id.Should().Be(1);
            recipe.Title.Should().Be("Test Recipe 1");
        }

        [Fact]
        public async Task GetRecipe_ReturnsNotFound_WhenInvalidIdProvided()
        {
            // Act
            var result = await _controller.GetRecipe(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task PostRecipe_CreatesRecipe_WhenValidDataProvided()
        {
            // Arrange
            var createDto = new CreateRecipeDto
            {
                Title = "New Test Recipe",
                Ingredients = new List<string> { "New Ingredient 1", "New Ingredient 2" },
                Steps = new List<string> { "New Step 1: Prepare everything", "New Step 2: Cook it all" },
                CookingTime = 25,
                DietaryTags = new List<string> { "Quick", "Healthy" }
            };

            // Act
            var result = await _controller.PostRecipe(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var recipe = createdResult.Value.Should().BeOfType<RecipeDto>().Subject;
            recipe.Title.Should().Be("New Test Recipe");
            recipe.CookingTime.Should().Be(25);

            // Verify it was actually saved
            var savedRecipe = await _context.Recipes.FindAsync(recipe.Id);
            savedRecipe.Should().NotBeNull();
            savedRecipe!.Title.Should().Be("New Test Recipe");
        }

        [Fact]
        public async Task PutRecipe_UpdatesRecipe_WhenValidDataProvided()
        {
            // Arrange
            var updateDto = new UpdateRecipeDto
            {
                Title = "Updated Test Recipe",
                CookingTime = 45
            };

            // Act
            var result = await _controller.PutRecipe(1, updateDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var recipe = okResult.Value.Should().BeOfType<RecipeDto>().Subject;
            recipe.Title.Should().Be("Updated Test Recipe");
            recipe.CookingTime.Should().Be(45);

            // Verify it was actually updated
            var updatedRecipe = await _context.Recipes.FindAsync(1);
            updatedRecipe!.Title.Should().Be("Updated Test Recipe");
            updatedRecipe.CookingTime.Should().Be(45);
        }

        [Fact]
        public async Task PutRecipe_ReturnsNotFound_WhenInvalidIdProvided()
        {
            // Arrange
            var updateDto = new UpdateRecipeDto { Title = "Updated Title" };

            // Act
            var result = await _controller.PutRecipe(999, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteRecipe_RemovesRecipe_WhenValidIdProvided()
        {
            // Act
            var result = await _controller.DeleteRecipe(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            // Verify it was actually deleted
            var deletedRecipe = await _context.Recipes.FindAsync(1);
            deletedRecipe.Should().BeNull();
        }

        [Fact]
        public async Task DeleteRecipe_ReturnsNotFound_WhenInvalidIdProvided()
        {
            // Act
            var result = await _controller.DeleteRecipe(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllDietaryTags_ReturnsDistinctTags()
        {
            // Act
            var result = await _controller.GetAllDietaryTags();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var tags = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
            tags.Should().Contain("Vegan");
            tags.Should().Contain("Vegetarian");
            tags.Should().Contain("Quick");
            tags.Should().HaveCount(3);
        }

        public void Dispose()
        {
            _context.Dispose();
            _controller.Dispose();
        }
    }
}
