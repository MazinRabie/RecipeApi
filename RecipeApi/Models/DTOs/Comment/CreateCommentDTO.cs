namespace RecipeApi.Models.DTOs.Comment
{
    public class CreateCommentDTO
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
