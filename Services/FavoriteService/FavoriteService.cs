using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plato_DB.Data;
using Plato_DB.Dtos.Other;
using Plato_DB.Interfaces;
using Plato_DB.Models;

namespace Plato_DB.Services.FavoriteService
{
    public class FavoriteService(AppDbContext context, IMapper mapper) : IFavoriteService
    {
        public async Task<bool> AddFavoriteAsync(Guid userId, Guid recipeId)
        {
            var alreadyFavorited = await context.Favorites
                .Include(r => r.User)
                .Include(r => r.Recipe)
                .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);

            if (alreadyFavorited)
                return false;

            var favorite = new Favorite
            {
                FavoriteId = Guid.NewGuid(),
                UserId = userId,
                RecipeId = recipeId,
                CreatedAt = DateTime.UtcNow
            };

            context.Favorites.Add(favorite);
            await context.SaveChangesAsync();

            var recipe = await context.Recipes
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            return true;
        }
        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId)
        {
            var favorite = await context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

            if (favorite == null)
                return false;

            context.Favorites.Remove(favorite);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<FavoriteDto>> GetFilteredFavoritesByUserAsync(
            FavoriteFilter favoriteFilter
        )
        {
            // Base query
            var query = context.Favorites
                .Where(f => f.UserId == favoriteFilter.UserId)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.User)
                .Include(f => f.User)
                .AsQueryable();

            // Optional filtering
            if (!string.IsNullOrWhiteSpace(favoriteFilter.Search))
            {
                query = query.Where(f =>
                    f.Recipe.Title.Contains(favoriteFilter.Search) ||
                    f.Recipe.Description.Contains(favoriteFilter.Search) ||
                    f.Recipe.User.Username.Contains(favoriteFilter.Search)
                );
            }

            // Count total before pagination
            var totalCount = await query.CountAsync();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalCount / (double)5);

            // Apply pagination
            var favorites = await query
                .OrderByDescending(f => f.CreatedAt) // ✅ optional sorting
                .Skip((favoriteFilter.Page - 1) * 5)
                .Take(5)
                .ToListAsync();

            // Map results
            var favoriteDtos = mapper.Map<List<FavoriteDto>>(favorites, opt =>
            {
                opt.Items["CurrentUserId"] = favoriteFilter.UserId;
            });

            // Build paged result
            return new PagedResult<FavoriteDto>
            {
                Items = favoriteDtos,
                CurrentPage = favoriteFilter.Page,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }
    }
}
