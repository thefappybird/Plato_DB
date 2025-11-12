using Plato_DB.Dtos.Other;

namespace Plato_DB.Services.RatingService
{
    public interface IRatingService
    {
        Task<bool> AddOrUpdateRatingAsync(Guid userId, Guid recipeId, CreateRatingDto ratingDto);
        Task<IEnumerable<RatingDto>> GetRatingsForRecipeAsync(Guid recipeId);
        Task<RatingDto?> GetUserRatingForRecipeAsync(Guid userId, Guid recipeId);
        Task<double> GetAverageRatingForRecipeAsync(Guid recipeId);
        Task<bool> DeleteRatingAsync(Guid userId, Guid recipeId);
    }
}
