namespace RecipeApi.Models.DTOs.User
{
    public class UpdateUserDTO
    {
        public string? Name { get; set; }
        public string? profilePicture { get; set; }
        public string? Gender { get; set; }
        public List<string>? Fridgeingredients { get; set; } = new List<string>();
    }
}
