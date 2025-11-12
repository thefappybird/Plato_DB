using AutoMapper;
using Plato_DB.Dtos.Other;
using Plato_DB.Dtos.Recipe;
using Plato_DB.Dtos.User;
using Plato_DB.Models;


namespace Plato_DB.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ---------------------------
            // USER ↔ DTO
            // ---------------------------
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Recipes, opt => opt.MapFrom(src => src.Recipes))
                .ForMember(dest => dest.Favorites, opt => opt.MapFrom(src => src.Favorites))
                .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings));

            // ---------------------------
            // RECIPE ↔ DTO
            // ---------------------------
            CreateMap<Recipe, GetRecipeDto>()
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Ratings.Count > 0 ? src.Ratings.Average(r => r.Score) : 0))
                .ForMember(dest => dest.TotalFavorites, opt => opt.MapFrom(src => src.Favorites.Count))
                .ForMember(dest => dest.IsFavoritedByCurrentUser, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    try
                    {
                        if (context.Items.TryGetValue("CurrentUserId", out var userIdObj) && userIdObj is Guid userId)
                        {
                            return src.Favorites.Any(f => f.UserId == userId);
                        }
                    }
                    catch { }

                    return false;
                }));
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Ratings.Count > 0 ? src.Ratings.Average(r => r.Score) : 0))
                .ForMember(dest => dest.TotalFavorites, opt => opt.MapFrom(src => src.Favorites.Count))
                .ForMember(dest => dest.IsFavoritedByCurrentUser, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    try
                    {
                        if (context.Items.TryGetValue("CurrentUserId", out var userIdObj) && userIdObj is Guid userId)
                        {
                            return src.Favorites.Any(f => f.UserId == userId);
                        }
                    }
                    catch { }

                    return false;
                }));
            CreateMap<Ingredient, IngredientDto>();
            CreateMap<Step, StepDto>();

            CreateMap<CreateRecipeDto, Recipe>()
                .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));

            CreateMap<UpdateRecipeDto, Recipe>()
                .ForMember(dest => dest.RecipeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore())
                .ForMember(dest => dest.Steps, opt => opt.Ignore());

            CreateMap<IngredientDto, Ingredient>()
                .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<StepDto, Step>()
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());


            CreateMap<Favorite, FavoriteDto>()
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Recipe, opt => opt.MapFrom(src => src.Recipe));

            // Rating <-> RatingDto
            CreateMap<Rating, RatingDto>()
                .ForMember(dest => dest.RecipeTitle, opt => opt.MapFrom(src => src.Recipe.Title))
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.User.Username));

        }
    }
}
