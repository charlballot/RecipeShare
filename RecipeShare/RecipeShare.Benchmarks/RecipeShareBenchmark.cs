using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeShare.Data;
using RecipeShare.DTOs;
using System.Net.Http.Json;

namespace RecipeShare.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 3, iterationCount: 5)]
    public class RecipeShareBenchmark
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [GlobalSetup]
        public async Task Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing DbContext
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<RecipeContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        // Add InMemory database for testing
                        services.AddDbContext<RecipeContext>(options =>
                            options.UseInMemoryDatabase("BenchmarkTestDb"));
                    });
                });

            _client = _factory.CreateClient();

            // Seed the database with test data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RecipeContext>();
            await DataSeeder.SeedAsync(context);

            // Add more recipes for better benchmarking
            await SeedAdditionalRecipes(context);
        }

        private async Task SeedAdditionalRecipes(RecipeContext context)
        {
            var recipes = new List<RecipeShare.Models.Recipe>();

            for (int i = 4; i <= 100; i++)
            {
                recipes.Add(new RecipeShare.Models.Recipe
                {
                    Title = $"Benchmark Recipe {i}",
                    Ingredients = new List<string> { $"Ingredient {i}A", $"Ingredient {i}B" },
                    Steps = new List<string> { $"Step 1 for recipe {i}", $"Step 2 for recipe {i}" },
                    CookingTime = 20 + (i % 60),
                    DietaryTags = new List<string> { i % 2 == 0 ? "Vegan" : "Vegetarian" }
                });
            }

            context.Recipes.AddRange(recipes);
            await context.SaveChangesAsync();
        }

        [Benchmark]
        public async Task GetAllRecipes()
        {
            var response = await _client.GetAsync("/api/recipes");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task GetRecipesWithDietaryFilter()
        {
            var response = await _client.GetAsync("/api/recipes?dietaryTag=Vegan");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task GetRecipesWithCookingTimeFilter()
        {
            var response = await _client.GetAsync("/api/recipes?maxCookingTime=30");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task GetRecipesWithTextSearch()
        {
            var response = await _client.GetAsync("/api/recipes?search=pasta");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task GetSingleRecipe()
        {
            var response = await _client.GetAsync("/api/recipes/1");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task CreateRecipe()
        {
            var newRecipe = new CreateRecipeDto
            {
                Title = "Benchmark Created Recipe",
                Ingredients = new List<string> { "Test Ingredient 1", "Test Ingredient 2" },
                Steps = new List<string> { "Test step 1 with enough characters", "Test step 2 with enough characters" },
                CookingTime = 25,
                DietaryTags = new List<string> { "Quick" }
            };

            var response = await _client.PostAsJsonAsync("/api/recipes", newRecipe);
            response.EnsureSuccessStatusCode();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
    }
}
