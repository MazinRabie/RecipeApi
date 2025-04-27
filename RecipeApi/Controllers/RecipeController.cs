using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeApi.Models;
using RecipeApi.Models.DTOs.Recipe;
using RecipeApi.Repository.IRepository;
using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase

    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper mapper;

        public RecipeController(IRecipeRepository recipeRepo, IUserRepository userRepo, IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _userRepo = userRepo;
            this.mapper = mapper;
        }
        // POST: /api/recipes/CreateRecipe
        [HttpPost("CreateRecipe")]
        public async Task<IActionResult> Create([FromBody] CreateRecipeDTO recipeDTO)
        {
            var recipe = mapper.Map<Recipe>(recipeDTO);
            var created = await _recipeRepo.CreateRecipeAsync(recipe);
            return Ok(created);
        }

        // GET: /api/recipes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var recipe = await _recipeRepo.GetRecipeByIdAsync(id);
            return recipe == null ? NotFound() : Ok(recipe);
        }

        // GET: /api/recipes/public
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicRecipes()
        {
            var recipes = await _recipeRepo.GetAllPublicRecipesAsync();
            return Ok(recipes);
        }

        // GET: /api/recipes/category/{category}
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory([FromRoute] string category)
        {
            var recipes = await _recipeRepo.GetRecipesByCategoryAsync(category);
            return Ok(recipes);
        }

        // GET: /api/recipes/search?query=pasta
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var results = await _recipeRepo.SearchRecipesAsync(query);
            return Ok(results);
        }

        // PUT: /api/recipes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeDTO updatedRecipe)
        {
            var result = await _recipeRepo.UpdateRecipeAsync(id, updatedRecipe);
            return result ? Ok("Recipe updated") : NotFound("Recipe not found");
        }

        // DELETE: /api/recipes/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _recipeRepo.DeleteRecipeAsync(id);
            return result ? Ok("Recipe deleted") : NotFound("Recipe not found");
        }
        [HttpPost("suggestions")]
        public async Task<IActionResult> GetSuggestedRecipes([FromBody] List<string> availableIngredients, [FromQuery] double threshold = 30)
        {
            if (availableIngredients == null || !availableIngredients.Any())
                return BadRequest("No ingredients provided.");

            var recipes = await _recipeRepo.GetRecipesByPartialIngredientsAsync(availableIngredients, threshold);

            if (recipes == null || recipes.Count == 0)
                return NotFound("No matching recipes found.");

            return Ok(recipes);
        }
        [HttpPost("Fridgesuggestions/{id}")]
        public async Task<IActionResult> GetSuggestedRecipes([FromRoute] int id, [FromQuery] double threshold = 30)
        {
            var availableIngredients = await _userRepo.GetFridgeingredientsAsync(id);
            if (availableIngredients == null || !availableIngredients.Any())
                return BadRequest("No ingredients provided.");

            var recipes = await _recipeRepo.GetRecipesByPartialIngredientsAsync(availableIngredients, threshold);

            if (recipes == null || recipes.Count == 0)
                return NotFound("No matching recipes found.");

            return Ok(recipes);
        }

        //[HttpPost("upload-with-recipe")]
        //public async Task<IActionResult> UploadWithRecipe([FromForm] IFormFile image, [FromForm] string recipeJson)
        //{
        //    if (image == null || image.Length == 0)
        //        return BadRequest("No image uploaded.");

        //    if (string.IsNullOrEmpty(recipeJson))
        //        return BadRequest("Recipe data missing.");

        //    // Deserialize recipe
        //    CreateRecipeDTO recipeDto;
        //    try
        //    {
        //        recipeDto = JsonConvert.DeserializeObject<CreateRecipeDTO>(recipeJson);
        //    }
        //    catch
        //    {
        //        return BadRequest("Invalid recipe JSON format.");
        //    }

        //    // Save image
        //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        //    var imagePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(imagePath, FileMode.Create))
        //    {
        //        await image.CopyToAsync(stream);
        //    }

        //    var imageUrl = $"/images/{uniqueFileName}";

        //    // Map DTO to model
        //    var recipe = mapper.Map<Recipe>(recipeDto);

        //    await _recipeRepo.CreateRecipeAsync(recipe);



        //    return Ok(recipe);
        //}

        [HttpPost("upload-with-recipe")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadWithRecipe([FromForm] RecipeUploadRequest request)
        {
            // Deserialize the recipe JSON
            var recipeDto = JsonConvert.DeserializeObject<CreateRecipeDTO>(request.RecipeJson);

            // Ensure directory exists
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            // Generate a unique filename using GUID
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
            var filePath = Path.Combine(imageDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            recipeDto.ImgURL = $"/images/{uniqueFileName}";

            // Save recipe to DB
            var recipe = mapper.Map<Recipe>(recipeDto);
            await _recipeRepo.CreateRecipeAsync(recipe);

            return Ok(new { message = "Recipe uploaded successfully", recipe });
        }



    }
}
public class RecipeUploadRequest
{
    [Required]
    public IFormFile Image { get; set; }

    [Required]
    public string RecipeJson { get; set; }
}
