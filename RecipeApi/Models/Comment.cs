namespace RecipeApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
