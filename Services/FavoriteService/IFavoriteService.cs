using Plato_DB.Dtos.Other;
using Plato_DB.Interfaces;

namespace Plato_DB.Services.FavoriteService
{
    public interface IFavoriteService
    {
        Task<bool> AddFavoriteAsync(Guid userId, Guid recipeId);
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId);
        Task<PagedResult<FavoriteDto>> GetFilteredFavoritesByUserAsync(FavoriteFilter favoriteFilter);

    }
}
