using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeApi.DataStore;
using RecipeApi.Models;
using RecipeApi.Repository.IRepository;

namespace RecipeApi.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public CommentRepository(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            _context = context;
        }
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId)
        {
            return await _context.Comments
                .Where(c => c.RecipeId == recipeId)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

