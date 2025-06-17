using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.DTOs;
using RecipeShare.Models;

namespace RecipeShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : Controller
    {
        private readonly RecipeContext _context;
        private readonly IMapper _mapper;

        public RecipesController(RecipeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes(
            [FromQuery] string? dietaryTag = null,
            [FromQuery] int? maxCookingTime = null,
            [FromQuery] string? search = null)
        {
            var query = _context.Recipes.AsQueryable();

            // Filter by dietary tag
            if (!string.IsNullOrEmpty(dietaryTag))
            {
                if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                {
                    query = query.Where(r => r.DietaryTags.Contains(dietaryTag));
                }
                else
                {
                    query = query.Where(r =>
                        _context.Recipes.FromSqlRaw("SELECT * FROM Recipes WHERE JSON_QUERY(DietaryTags) LIKE {0}", $"%{dietaryTag}%").Contains(r));
                }
            }

            // Filter by maximum cooking time
            if (maxCookingTime.HasValue)
            {
                query = query.Where(r => r.CookingTime <= maxCookingTime.Value);
            }

            // Search in title and ingredients
            if (!string.IsNullOrEmpty(search))
            {
                if (_context.Database.ProviderName == "")
                {
                    query = query.Where(r =>
                        EF.Functions.Like(r.Title.ToLower(), $"%{search.ToLower()}%") ||
                        r.Ingredients.Contains(search));
                }
                else
                {
                    query = query.Where(r =>
                        EF.Functions.Like(r.Title.ToLower(), $"%{search.ToLower()}%") ||
                        _context.Recipes.FromSqlRaw("SELECT * FROM Recipes WHERE JSON_QUERY(Ingredients) LIKE {0}", $"%{search}%").Contains(r));
                }
            }

            var recipes = await query.OrderBy(r => r.Title).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<RecipeDto>>(recipes));
        }

        // GET: Recipes/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound($"Recipe with ID {id} not found.");
            }

            return Ok(_mapper.Map<RecipeDto>(recipe));
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, UpdateRecipeDto updateRecipeDto)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound($"Recipe with ID {id} not found.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateRecipeDto.Title))
                recipe.Title = updateRecipeDto.Title;

            if (updateRecipeDto.Ingredients != null)
                recipe.Ingredients = updateRecipeDto.Ingredients;

            if (updateRecipeDto.Steps != null)
                recipe.Steps = updateRecipeDto.Steps;

            if (updateRecipeDto.CookingTime.HasValue)
                recipe.CookingTime = updateRecipeDto.CookingTime.Value;

            if (updateRecipeDto.DietaryTags != null)
                recipe.DietaryTags = updateRecipeDto.DietaryTags;

            recipe.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(_mapper.Map<RecipeDto>(recipe));
        }

        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> PostRecipe(CreateRecipeDto createRecipeDto)
        {
            var recipe = _mapper.Map<Recipe>(createRecipeDto);

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            var recipeDto = _mapper.Map<RecipeDto>(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipeDto);
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound($"Recipe with ID {id} not found.");
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Recipes/tags
        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllDietaryTags()
        {
            var allTags = _context.Recipes
                .AsEnumerable()
                .SelectMany(r => r.DietaryTags)
                .Distinct()
                .OrderBy(tag => tag)
                .ToList();

            return Ok(allTags);
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
