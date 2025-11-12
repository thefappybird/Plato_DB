using Plato_DB.Dtos.Recipe;
using Plato_DB.Dtos.Other;
using Plato_DB.Interfaces;

namespace Plato_DB.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<PagedResult<RecipeDto>?> GetFilteredRecipes(RecipeFilter recipeFilter,Guid? currentUserId);
        Task<PagedResult<RecipeDto>> GetDashRecipes(Guid? currentUserId);
        Task<GetRecipeDto?> GetRecipe(Guid recipeId, Guid? currentUserId);
        Task<string?> CreateRecipe (CreateRecipeDto newRecipe, Guid userId);
        Task<string?> UpdateRecipe(Guid recipeId, UpdateRecipeDto updatedRecipe, Guid userId);
        Task<string?> DeleteRecipe(Guid recipeId, Guid userId);
    }
}
