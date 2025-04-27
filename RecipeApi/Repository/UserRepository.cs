using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeApi.DataStore;
using RecipeApi.Models;
using RecipeApi.Models.DTOs.User;
using RecipeApi.Repository.IRepository;

namespace RecipeApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }


        public async Task<List<User>> GetAllusersAsync()
        {
            var users = await _context.Users.Include(x => x.Favorites).Include(x => x.Recipes).ToListAsync();
            return users;
        }

        // 1. Create user (register)
        public async Task<(bool IsSuccess, string Message)> CreateUserAsync(CreateUserDTO userDto)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (exists)
                return (false, "Email already in use.");
            if (userDto.Password != userDto.ConfirmPassword) return (false, "passwords are not matched");
            var user = mapper.Map<User>(userDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, "User created successfully.");
        }

        // 2. Get user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // 3. Get user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                .Include(u => u.Recipes)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // 4. Check password 
        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && user.Password == password;
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDTO updatedUser)
        {
            var existingUser = await GetUserByIdAsync(userId);
            if (existingUser == null)
                return false;
            //existingUser=mapper.Map<User>(updatedUser);
            mapper.Map(updatedUser, existingUser);
            _context.Update(existingUser);
            await _context.SaveChangesAsync();

            return true;
        }
        // 6. Delete user
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // 7. Add favorite
        public async Task<bool> AddFavoriteAsync(int userId, int recipeId)
        {
            var user = await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var recipe = await _context.Recipes.FindAsync(recipeId);

            if (user == null || recipe == null || user.Favorites.Contains(recipe))
                return false;

            user.Favorites.Add(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        // 8. Remove favorite
        public async Task<bool> RemoveFavoriteAsync(int userId, int recipeId)
        {
            var user = await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var recipe = await _context.Recipes.FindAsync(recipeId);

            if (user == null || recipe == null || !user.Favorites.Contains(recipe))
                return false;

            user.Favorites.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        // 9. Get user favorites
        public async Task<List<Recipe>> GetUserFavoritesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Favorites ?? new List<Recipe>();
        }

        // 10. Get recipes created by user
        public async Task<List<Recipe>> GetUserRecipesAsync(int userId)
        {
            return await _context.Recipes
                .Where(r => r.AuthorId == userId)
                .ToListAsync();
        }


        public async Task<bool> AddFridgeingredientsAsync(int userId, List<string> ingredients)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;


            var merged = new HashSet<string>((user.Fridgeingredients == null) ? new List<string>() : user.Fridgeingredients);
            merged.UnionWith(ingredients);
            ;
            user.Fridgeingredients = merged.Select(x => x.ToLower()).ToList();
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteFridgeingredientsAsync(int userId, string ingredient)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            var userFridge = user.Fridgeingredients;
            userFridge.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateFridgeingredientsAsync(int userId, List<string> ingredients)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            user.Fridgeingredients = ingredients;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>>? GetFridgeingredientsAsync(int userId)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;
            var userFridge = user.Fridgeingredients;
            return userFridge;
        }



    }
}
