using RecipeShare.Models;
using Microsoft.EntityFrameworkCore;

namespace RecipeShare.Data
{
    public class DataSeeder
    {
        public static async Task SeedAsync(RecipeContext context)
        {
            if (await context.Recipes.AnyAsync())
                return; // Database already seeded

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Title = "Classic Spaghetti Carbonara",
                    Ingredients = new List<string>
                    {
                        "400g spaghetti",
                        "200g pancetta or guanciale, diced",
                        "4 large eggs",
                        "100g Pecorino Romano cheese, grated",
                        "50g Parmesan cheese, grated",
                        "Black pepper to taste",
                        "Salt for pasta water"
                    },
                    Steps = new List<string>
                    {
                        "Bring a large pot of salted water to boil and cook spaghetti according to package directions",
                        "While pasta cooks, heat a large skillet over medium heat and cook pancetta until crispy",
                        "In a bowl, whisk together eggs, both cheeses, and black pepper",
                        "Drain pasta, reserving 1 cup pasta water",
                        "Add hot pasta to the skillet with pancetta",
                        "Remove from heat and quickly toss with egg mixture, adding pasta water as needed",
                        "Serve immediately with extra cheese and black pepper"
                    },
                    CookingTime = 25,
                    DietaryTags = new List<string> { "Italian", "Quick", "Comfort Food" }
                },
                new Recipe
                {
                    Title = "Quinoa Buddha Bowl",
                    Ingredients = new List<string>
                    {
                        "1 cup quinoa",
                        "2 cups vegetable broth",
                        "1 sweet potato, cubed",
                        "1 cup chickpeas, cooked",
                        "2 cups baby spinach",
                        "1 avocado, sliced",
                        "1/4 cup pumpkin seeds",
                        "2 tbsp tahini",
                        "1 tbsp lemon juice",
                        "1 tsp maple syrup",
                        "Salt and pepper to taste"
                    },
                    Steps = new List<string>
                    {
                        "Preheat oven to 400°F (200°C)",
                        "Toss cubed sweet potato with oil, salt, and pepper; roast for 25 minutes",
                        "Cook quinoa in vegetable broth according to package directions",
                        "Heat chickpeas in a pan with spices of choice",
                        "Whisk together tahini, lemon juice, maple syrup, and water for dressing",
                        "Assemble bowls with quinoa, roasted sweet potato, chickpeas, spinach, and avocado",
                        "Drizzle with tahini dressing and sprinkle with pumpkin seeds"
                    },
                    CookingTime = 35,
                    DietaryTags = new List<string> { "Vegan", "Gluten-Free", "Healthy", "High-Protein" }
                },
                new Recipe
                {
                    Title = "Beef Stir-Fry with Vegetables",
                    Ingredients = new List<string>
                    {
                        "500g beef sirloin, sliced thin",
                        "2 tbsp soy sauce",
                        "1 tbsp cornstarch",
                        "2 tbsp vegetable oil",
                        "1 bell pepper, sliced",
                        "1 cup broccoli florets",
                        "1 carrot, sliced",
                        "2 cloves garlic, minced",
                        "1 tsp fresh ginger, grated",
                        "2 tbsp oyster sauce",
                        "1 tbsp sesame oil",
                        "Green onions for garnish"
                    },
                    Steps = new List<string>
                    {
                        "Marinate sliced beef with soy sauce and cornstarch for 15 minutes",
                        "Heat oil in a wok or large skillet over high heat",
                        "Stir-fry beef until browned, about 3-4 minutes; remove and set aside",
                        "Add more oil if needed and stir-fry vegetables until crisp-tender",
                        "Add garlic and ginger, stir-fry for 30 seconds",
                        "Return beef to pan, add oyster sauce and sesame oil",
                        "Toss everything together and serve with rice, garnished with green onions"
                    },
                    CookingTime = 20,
                    DietaryTags = new List<string> { "Asian", "High-Protein", "Quick", "Low-Carb" }
                }
            };

            context.Recipes.AddRange(recipes);
            await context.SaveChangesAsync();
        }
    }
}
