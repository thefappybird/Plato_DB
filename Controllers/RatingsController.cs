using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato_DB.Dtos.Other;
using Plato_DB.Services.RatingService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Plato_DB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // all routes require valid token
    public class RatingController(IRatingService ratingService) : ControllerBase
    {

        // ✅ Helper: Extract UserId from JWT claims
        private Guid GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirst("sub");

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

            return userId;
        }

        // ✅ POST: Add or update rating
        [HttpPost("{recipeId:guid}")]
        public async Task<IActionResult> AddOrUpdate(Guid recipeId, [FromForm] CreateRatingDto dto)
        {
            var userId = GetUserIdFromClaims();

            var result = await ratingService.AddOrUpdateRatingAsync(userId, recipeId, dto);
            if (result == false)
            {
                return Ok("Successfully Updated Rating.");
            }
            return Ok("Successfully Added Rating.");
        }

        // ✅ GET: All ratings for a recipe
        [AllowAnonymous] // optional — depends on your app rules
        [HttpGet("{recipeId:guid}")]
        public async Task<IActionResult> GetRatingsForRecipe(Guid recipeId)
        {
            var ratings = await ratingService.GetRatingsForRecipeAsync(recipeId);
            return Ok(ratings);
        }

        // ✅ GET: Current user's rating for this recipe
        [HttpGet("{recipeId:guid}/user")]
        public async Task<IActionResult> GetUserRatingForRecipe(Guid recipeId)
        {
            var userId = GetUserIdFromClaims();
            var rating = await ratingService.GetUserRatingForRecipeAsync(userId, recipeId);

            if (rating == null)
                return NotFound();

            return Ok(rating);
        }

        // ✅ GET: Average rating for a recipe
        [AllowAnonymous]
        [HttpGet("{recipeId:guid}/average")]
        public async Task<IActionResult> GetAverage(Guid recipeId)
        {
            var average = await ratingService.GetAverageRatingForRecipeAsync(recipeId);
            return Ok(new { recipeId, average });
        }

        // ✅ DELETE: Remove user’s rating for this recipe
        [HttpDelete("{recipeId:guid}")]
        public async Task<IActionResult> Delete(Guid recipeId)
        {
            var userId = GetUserIdFromClaims();
            var success = await ratingService.DeleteRatingAsync(userId, recipeId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
