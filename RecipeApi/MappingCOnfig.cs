using AutoMapper;
using RecipeApi.Models;
using RecipeApi.Models.DTOs.Comment;
using RecipeApi.Models.DTOs.Recipe;
using RecipeApi.Models.DTOs.User;

namespace RecipeApi
{
    public class MappingCOnfig : Profile
    {

        public MappingCOnfig()
        {
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Recipe, CreateRecipeDTO>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Recipe, UpdateRecipeDTO>()
                .ReverseMap()
                .ForMember(dest => dest.Ingredients, opt =>
                    opt.PreCondition(src => src.Ingredients != null && src.Ingredients.Count > 0)).ForMember(dest => dest.CookingSteps, opt =>
                    opt.PreCondition(src => src.CookingSteps != null && src.CookingSteps.Count > 0)).
                    ForMember(dest => dest.Rating, opt =>
        opt.PreCondition(src => src.Rating != null))
                .ForAllOtherMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Comment, CreateCommentDTO>().ReverseMap();
        }
    }
}
