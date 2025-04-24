using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeApi.DataStore;
using RecipeApi.Models;
using RecipeApi.Models.DTOs.Recipe;
using RecipeApi.Repository.IRepository;

namespace RecipeApi.Repository
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public RecipeRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }


        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Comments)
                .Include(r => r.author)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetAllPublicRecipesAsync()
        {
            return await _context.Recipes
                .Where(r => (bool)r.IsPublic)
                .Include(r => r.author)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            return await _context.Recipes
                .Where(r => r.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<bool> UpdateRecipeAsync(int Id, UpdateRecipeDTO updatedRecipe)
        {
            var existing = await _context.Recipes.FindAsync(Id);
            if (existing == null) return false;

            mapper.Map(updatedRecipe, existing);
            _context.Update(existing);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.FavoritedBy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return false;

            // 🔑 Step 1: Clear the many-to-many relationship
            recipe.FavoritedBy.Clear();

            // Optional: If you also want to delete comments, include and remove them too
            var comments = _context.Comments.Where(c => c.RecipeId == id);
            _context.Comments.RemoveRange(comments);

            // Step 2: Remove the recipe itself
            _context.Recipes.Remove(recipe);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<Recipe>> SearchRecipesAsync(string keyword)
        {
            return await _context.Recipes
                .Where(r =>
                    r.Name.Contains(keyword) ||
                    r.Description.Contains(keyword) ||
                    r.Country.Contains(keyword))
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecipesByPartialIngredientsAsync(List<string> availableIngredients, double thresholdPercent = 30)
        {
            var recipes = await GetAllPublicRecipesAsync();
            var matchedRecipes = new List<Recipe>();

            foreach (var recipe in recipes)
            {
                if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
                    continue;

                int matchCount = recipe.Ingredients
                    .Count(ingredient => availableIngredients
                        .Any(i => string.Equals(i.Trim(), ingredient.Trim(), StringComparison.OrdinalIgnoreCase)));

                double matchPercent = (matchCount / (double)recipe.Ingredients.Count) * 100.0;

                if (matchPercent >= thresholdPercent)
                {
                    matchedRecipes.Add(recipe);
                }
            }

            return matchedRecipes;
        }

    }
}
