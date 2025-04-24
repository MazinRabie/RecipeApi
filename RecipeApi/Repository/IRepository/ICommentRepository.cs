using RecipeApi.Models;

namespace RecipeApi.Repository.IRepository
{
    public interface ICommentRepository
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId);
    }
}