using Microsoft.AspNetCore.Mvc;
using RecipeApi.Models.DTOs.User;
using RecipeApi.Repository.IRepository;

namespace RecipeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // POST: /api/users
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO user)
        {
            var result = await _userRepo.CreateUserAsync(user);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        // GET: /api/users/{id}
        [HttpGet("Getuser/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        // GET: /api/users/Getuser?email=test@test.com
        [HttpGet("Getuser")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            return user == null ? NotFound() : Ok(user);
        }

        // POST: /api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Credintials credintials)
        {
            var success = await _userRepo.CheckPasswordAsync(credintials.Email, credintials.Password);
            if (success)
            {
                var account = await _userRepo.GetUserByEmailAsync(credintials.Email);
                return Ok(account.Id);

            }
            return Unauthorized("Invalid credentials");
        }
        [HttpGet("getUsers")]
        public async Task<IActionResult> getAllusersAsync()
        {
            return Ok(await _userRepo.GetAllusersAsync());
        }

        // POST: /api/users/{id}/favorites/{recipeId}
        [HttpPost("{id}/favorites/{recipeId}")]
        public async Task<IActionResult> AddFavorite([FromRoute] int id, [FromRoute] int recipeId)
        {
            var result = await _userRepo.AddFavoriteAsync(id, recipeId);
            return result ? Ok("Added to favorites") : BadRequest("Could not add favorite");
        }

        // DELETE: /api/users/{id}/favorites/{recipeId}
        [HttpDelete("{id}/favorites/{recipeId}")]
        public async Task<IActionResult> RemoveFavorite(int id, int recipeId)
        {
            var result = await _userRepo.RemoveFavoriteAsync(id, recipeId);
            return result ? Ok("Removed from favorites") : BadRequest("Could not remove favorite");
        }

        // GET: /api/users/{id}/favorites
        [HttpGet("{id}/favorites")]
        public async Task<IActionResult> GetFavorites(int id)
        {
            var recipes = await _userRepo.GetUserFavoritesAsync(id);
            return Ok(recipes);
        }

        // GET: /api/users/{id}/recipes
        [HttpGet("{id}/recipes")]
        public async Task<IActionResult> GetUserRecipes(int id)
        {
            var recipes = await _userRepo.GetUserRecipesAsync(id);
            return Ok(recipes);
        }

        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updatedUser)
        {

            var result = await _userRepo.UpdateUserAsync(id, updatedUser);
            return result ? Ok("User updated") : NotFound("User not found");
        }

        [HttpPost("AddToFridge/{id}")]
        public async Task<IActionResult> AddToFridge([FromRoute] int id, [FromBody] List<string> ingredients)
        {
            var IsSucc = await _userRepo.AddFridgeingredientsAsync(id, ingredients);
            return Ok(IsSucc);
        }
        [HttpPut("UpdateFridge/{id}")]
        public async Task<IActionResult> UpadteFridge([FromRoute] int id, [FromBody] List<string> ingredients)
        {
            var IsSucc = await _userRepo.UpdateFridgeingredientsAsync(id, ingredients);
            return Ok(IsSucc);
        }
        [HttpDelete("DeleteFromFridge/{id}")]
        public async Task<IActionResult> UpadteFridge([FromRoute] int id, [FromBody] string ingredient)
        {
            var IsSucc = await _userRepo.DeleteFridgeingredientsAsync(id, ingredient);
            return Ok(IsSucc);
        }
        [HttpGet("GetFromFridge/{id}")]
        public async Task<IActionResult> UpadteFridge([FromRoute] int id)
        {
            var fridgeIngredients = await _userRepo.GetFridgeingredientsAsync(id);
            return Ok(fridgeIngredients);
        }

    }


    public class Credintials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

