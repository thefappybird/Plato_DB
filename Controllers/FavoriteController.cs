using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato_DB.Interfaces;
using Plato_DB.Services.FavoriteService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Plato_DB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoriteController(IFavoriteService favoriteService) : ControllerBase
    {
        // ✅ Extract UserId from JWT
        private Guid GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirst("sub");

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

            return userId;
        }

        // ✅ POST: Add recipe to favorites
        [HttpPost("{recipeId:guid}")]
        public async Task<IActionResult> AddFavorite(Guid recipeId)
        {
            var userId = GetUserIdFromClaims();
            var favorite = await favoriteService.AddFavoriteAsync(userId, recipeId);
            if(favorite == false)
            {
                return BadRequest("This recipe is already in your favorites.");
            }
            return Ok("Sucessfully added to your favorites.");
        }

        // ✅ DELETE: Remove favorite
        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> RemoveFavorite(Guid recipeId)
        {
            var userId = GetUserIdFromClaims();
            var success = await favoriteService.RemoveFavoriteAsync(userId, recipeId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        // ✅ GET: All current user favorites
        [HttpPost("current")]
        public async Task<IActionResult> GetUserFavorites([FromForm] FavoriteFilterBase favoriteFilterBase)
        {
            var userId = GetUserIdFromClaims();
            var favoriteFilter = new FavoriteFilter
            {
                UserId = userId,
                Page = favoriteFilterBase.Page,
                Search = favoriteFilterBase.Search
            };
            var favorites = await favoriteService.GetFilteredFavoritesByUserAsync(favoriteFilter);
            return Ok(favorites);
        }
    }
}
