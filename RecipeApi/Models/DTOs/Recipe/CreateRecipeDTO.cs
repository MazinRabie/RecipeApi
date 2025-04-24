namespace RecipeApi.Models.DTOs.Recipe
{
    public class CreateRecipeDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Ingredients { get; set; }
        public string? ImgURL { get; set; }
        public string? Category { get; set; }
        public int AuthorId { get; set; }
        public bool IsPublic { get; set; }
        public string? Country { get; set; }
        public List<string?>? CookingSteps { get; set; }
    }
}
