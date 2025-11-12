using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato_DB.Dtos.Other;
using Plato_DB.Dtos.Recipe;
using Plato_DB.Interfaces;
using Plato_DB.Services.RecipeService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Plato_DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RecipeController(IRecipeService recipeService) : ControllerBase
    {
        private Guid? GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirst("sub");

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                return null;

            return userId;
        }
        [HttpGet("dash-recipes")]
        public async Task<ActionResult<PagedResult<RecipeDto>>> GetAllRecipes()
        {
            var response = await recipeService.GetDashRecipes(GetUserIdFromClaims());
            // Logic to retrieve all recipes
            return Ok(response);
        }
        [HttpPost("get-filtered-recipes")]
        public async Task<ActionResult<PagedResult<RecipeDto>>> GetFilteredRecipes (
            [FromForm] RecipeFilter recipeFilter)
        {
            var response = await recipeService.GetFilteredRecipes(recipeFilter, GetUserIdFromClaims());
            if (response == null)
                return BadRequest("No recipes found with the given filters.");
            return Ok(response);
        }
        [HttpGet("get-recipe/{recipeId}")]
        public async Task<ActionResult<GetRecipeDto>> GetRecipeById(Guid recipeId)
        {
            var response = await recipeService.GetRecipe(recipeId, GetUserIdFromClaims());
            if (response == null)
                return BadRequest("Recipe not found.");
            return Ok(response);
        }
        [Authorize]
        [HttpPost("auth/create-recipe")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<string>> CreateRecipe([FromForm] CreateRecipeDto newRecipe)
        {
            var form = await Request.ReadFormAsync();
            foreach (var key in form.Keys)
            {
                Console.WriteLine($"{key}: {form[key]}");
            }
            // Extract user ID from JWT token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirst("sub");

            if (idClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(idClaim.Value, out var userId))
                return Unauthorized();

            var response = await recipeService.CreateRecipe(newRecipe, userId);

            if (response == null)
                return BadRequest("Failed to create recipe.");
            return Ok(response);
        }
        [Authorize]
        [HttpPut("auth/update-recipe/{recipeId}")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<string?>> UpdateRecipe(Guid recipeId, [FromForm] UpdateRecipeDto updatedRecipe)
        {
            // Extract user ID from JWT token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirst("sub");
            if (idClaim == null)
                return Unauthorized();
            if (!Guid.TryParse(idClaim.Value, out var userId))
                return Unauthorized();
            var response = await recipeService.UpdateRecipe(recipeId, updatedRecipe, userId);
            if (response == null)
                return BadRequest("Failed to update recipe or you are not the author.");
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("auth/delete-recipe/{recipeId}")]
        public async Task<ActionResult<string?>> DeleteRecipe(Guid recipeId)
        {
            // Extract user ID from JWT token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirst("sub");
            if (idClaim == null)
                return Unauthorized();
            if (!Guid.TryParse(idClaim.Value, out var userId))
                return Unauthorized();
            var response = await recipeService.DeleteRecipe(recipeId, userId);
            if (response == null)
                return BadRequest("Failed to delete recipe or you are not the author.");
            return Ok(response);
        }
    }
}
