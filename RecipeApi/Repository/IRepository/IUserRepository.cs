using RecipeApi.Models;
using RecipeApi.Models.DTOs.User;

namespace RecipeApi.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllusersAsync();
        Task<bool> AddFavoriteAsync(int userId, int recipeId);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<(bool IsSuccess, string Message)> CreateUserAsync(CreateUserDTO userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<Recipe>> GetUserFavoritesAsync(int userId);
        Task<List<Recipe>> GetUserRecipesAsync(int userId);
        Task<bool> RemoveFavoriteAsync(int userId, int recipeId);
        Task<bool> UpdateUserAsync(int userId, UpdateUserDTO updatedUser);
        Task<bool> AddFridgeingredientsAsync(int userId, List<string> ingredients);
        Task<bool> UpdateFridgeingredientsAsync(int userId, List<string> ingredients);
        Task<bool> DeleteFridgeingredientsAsync(int userId, string ingredient);
        Task<List<string>> GetFridgeingredientsAsync(int userId);

    }
}