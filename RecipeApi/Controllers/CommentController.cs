using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.Models;
using RecipeApi.Models.DTOs.Comment;
using RecipeApi.Repository.IRepository;

namespace RecipeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IMapper mapper;

        public CommentController(ICommentRepository commentRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            this.mapper = mapper;
        }
        // POST: /api/comments
        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDTO commentDTO)
        {
            var comment = mapper.Map<Comment>(commentDTO);
            var added = await _commentRepo.AddCommentAsync(comment);
            return Ok(added);
        }

        // GET: /api/comments/recipe/{recipeId}
        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetByRecipe([FromRoute] int recipeId)
        {
            var comments = await _commentRepo.GetCommentsByRecipeIdAsync(recipeId);
            return Ok(comments);
        }

        // DELETE: /api/comments/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete(int commentId)
        {
            var result = await _commentRepo.DeleteCommentAsync(commentId);
            return result ? Ok("Comment deleted") : NotFound("Comment not found");
        }
    }
}
