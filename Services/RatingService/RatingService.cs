using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plato_DB.Data;
using Plato_DB.Dtos.Other;
using Plato_DB.Models;

namespace Plato_DB.Services.RatingService
{
    public class RatingService(AppDbContext context, IMapper mapper) : IRatingService
    {
        public async Task<bool> AddOrUpdateRatingAsync(Guid userId, Guid recipeId, CreateRatingDto dto)
        {
            var rating = await context.Ratings
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);
            var response = true;
            if (rating == null)
            {
                rating = new Rating
                {
                    RatingId = Guid.NewGuid(),
                    UserId = userId,
                    RecipeId = recipeId,
                    Score = dto.Score,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };
                context.Ratings.Add(rating);
            }
            else
            {
                rating.Score = dto.Score;
                rating.Comment = dto.Comment;
                rating.CreatedAt = DateTime.UtcNow;
                response = false;
            }

            await context.SaveChangesAsync();
            return response;
        }
        public async Task<IEnumerable<RatingDto>> GetRatingsForRecipeAsync(Guid recipeId)
        {
            var ratings = await context.Ratings
                .Include(r => r.User)
                .Include(r => r.Recipe)
                .Where(r => r.RecipeId == recipeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return mapper.Map<IEnumerable<RatingDto>>(ratings);
        }
        public async Task<RatingDto?> GetUserRatingForRecipeAsync(Guid userId, Guid recipeId)
        {
            var rating = await context.Ratings
                .Include(r => r.User)
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);

            return rating == null ? null : mapper.Map<RatingDto>(rating);
        }
        public async Task<double> GetAverageRatingForRecipeAsync(Guid recipeId)
        {
            return await context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .AverageAsync(r => (double?)r.Score) ?? 0.0;
        }

        // ✅ Delete rating (user can only delete their own)
        public async Task<bool> DeleteRatingAsync(Guid userId, Guid recipeId)
        {
            var rating = await context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);

            if (rating == null)
                return false;

            context.Ratings.Remove(rating);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
