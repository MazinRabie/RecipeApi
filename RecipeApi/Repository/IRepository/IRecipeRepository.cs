using RecipeApi.Models;
using RecipeApi.Models.DTOs.Recipe;

namespace RecipeApi.Repository.IRepository
{
    public interface IRecipeRepository
    {
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(int id);
        Task<List<Recipe>> GetAllPublicRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(int id);
        Task<List<Recipe>> GetRecipesByCategoryAsync(string category);
        Task<List<Recipe>> SearchRecipesAsync(string keyword);
        Task<bool> UpdateRecipeAsync(int Id, UpdateRecipeDTO updatedRecipe);
        Task<List<Recipe>> GetRecipesByPartialIngredientsAsync(List<string> availableIngredients, double thresholdPercent);
    }
}