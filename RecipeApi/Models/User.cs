namespace RecipeApi.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? profilePicture { get; set; }
        public string Gender { get; set; }
        public List<string>? Fridgeingredients { get; set; } = new List<string>();

        public List<Recipe> Favorites { get; set; } = new List<Recipe>();
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
