using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Plato_DB.Data;
using Plato_DB.Dtos.Other;
using Plato_DB.Dtos.Recipe;
using Plato_DB.Interfaces;
using Plato_DB.Models;

namespace Plato_DB.Services.RecipeService
{
    public class RecipeService(AppDbContext context, IMapper mapper, Cloudinary cloudinary) : IRecipeService
    {
        public async Task<PagedResult<RecipeDto>?> GetFilteredRecipes(RecipeFilter recipeService, Guid? currentUserId=null)
        {
            var query = context.Recipes
                .Include(r => r.User)
                .Include(r => r.Ratings)
                .Include(r => r.Favorites)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .AsQueryable();

            // 🧠 Apply filters dynamically
            if (!string.IsNullOrEmpty(recipeService.title))
                query = query.Where(r => r.Title.Contains(recipeService.title));

            if (!string.IsNullOrEmpty(recipeService.authorName))
                query = query.Where(r => r.User.Username.Contains(recipeService.authorName));

            query = recipeService.sort?.ToLower() switch
            {
                "title_asc" => query.OrderBy(r => r.Title),
                "title_desc" => query.OrderByDescending(r => r.Title),
                "date_asc" => query.OrderBy(r => r.CreatedAt),
                "date_desc" => query.OrderByDescending(r => r.CreatedAt),
                "rating_asc" => query.OrderBy(r => r.Ratings.Average(rt => (double?)rt.Score) ?? 0),
                "rating_desc" => query.OrderByDescending(r => r.Ratings.Average(rt => (double?)rt.Score) ?? 0),
                _ => query.OrderByDescending(r => r.CreatedAt) // default
            };

            // 🧾 Pagination logic
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)5);
            int currentPage = recipeService.page;
            if (totalPages == 0) currentPage = 1;
            else if (currentPage > totalPages) currentPage = totalPages;

            var skip = (recipeService.page - 1) * 5;

            var pagedRecipes = await query
                .Skip(skip)
                .Take(5)
                .ToListAsync();

            // 🧠 Map to DTO
            var recipeDtos = mapper.Map<List<RecipeDto>>(pagedRecipes, opt =>
            {
                opt.Items["CurrentUserId"] = currentUserId;
            });

            return new PagedResult<RecipeDto>
            {
                Items = recipeDtos,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }
        public async Task<PagedResult<RecipeDto>> GetDashRecipes(Guid? currentUserId)
        {
            var query = context.Recipes
                .Include(r => r.User)
                .Include(r => r.Ratings)
                .Include(r => r.Favorites)
                .Include(r => r.Ingredients)
                .OrderByDescending(r => r.Ratings.Any()
                    ? r.Ratings.Average(rt => (double?)rt.Score) ?? 0
                    : 0);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / 5.0);

            var pagedRecipes = await query
                .Skip(0) // Always page 1
                .Take(5)
                .ToListAsync();

            var recipeDtos = mapper.Map<List<RecipeDto>>(pagedRecipes, opt =>
            {
                opt.Items["CurrentUserId"] = currentUserId;
            });

            return new PagedResult<RecipeDto>
            {
                Items = recipeDtos,
                CurrentPage = 1,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }
        public async Task<GetRecipeDto?> GetRecipe(Guid recipeId, Guid? currentUserId)
        {
            var recipe = await context.Recipes
               .Include(r => r.User)
               .Include(r => r.Ingredients)
               .Include(r => r.Steps)
               .Include(r => r.Ratings)
               .Include(r => r.Favorites)
               .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null)
                return null;

            // Convert to DTO
            var currentRecipe = mapper.Map<GetRecipeDto>(recipe, opt =>
            {
                opt.Items["CurrentUserId"] = currentUserId;
            });
 
            return currentRecipe;
        }
        
        public async Task<string?> CreateRecipe(CreateRecipeDto newRecipe, Guid userId)
        {
            if (newRecipe == null)
                return "Invalid recipe data.";

            var recipe = mapper.Map<Recipe>(newRecipe);

            if (newRecipe.Image != null && newRecipe.Image.Length > 0)
            {
                using var stream = newRecipe.Image.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(newRecipe.Image.FileName, stream),
                    Folder = "recipes", // optional Cloudinary folder
                    Transformation = new Transformation().Width(800).Height(800).Crop("fill").Gravity("auto")
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                recipe.ImageUrl = uploadResult.SecureUrl?.ToString();
            }
            recipe.UserId = userId;

            context.Recipes.Add(recipe);
            await context.SaveChangesAsync();

            return "Recipe Created Successfully.";
        }
        public async Task<string?> UpdateRecipe(Guid recipeId, UpdateRecipeDto updatedRecipe, Guid userId)
        {
            var recipe = await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            if (recipe == null)
                return null;

            // ✅ Update scalar fields (Title, Description, etc.)
            mapper.Map(updatedRecipe, recipe);

            // ✅ Handle image upload
            if (updatedRecipe.Image != null && updatedRecipe.Image.Length > 0)
            {
                using var stream = updatedRecipe.Image.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updatedRecipe.Image.FileName, stream),
                    Folder = "recipes",
                    Transformation = new Transformation().Width(800).Height(800).Crop("fill").Gravity("auto")
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                recipe.ImageUrl = uploadResult.SecureUrl.ToString();
            }

            // ✅ Replace Ingredients & Steps
            context.Ingredients.RemoveRange(recipe.Ingredients);
            context.Steps.RemoveRange(recipe.Steps);

            recipe.Ingredients = updatedRecipe.Ingredients.Select(i => new Ingredient
            {
                RecipeId = recipe.RecipeId,
                Name = i.Name,
                Amount = i.Amount,
                Units = i.Units
            }).ToList();

            recipe.Steps = updatedRecipe.Steps.Select(s => new Step
            {
                RecipeId = recipe.RecipeId,
                StepNumber = s.StepNumber,
                Description = s.Description
            }).ToList();

            await context.SaveChangesAsync();

            recipe = await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            mapper.Map<RecipeDto>(recipe);

            return "Recipe Edited Successfully.";
        }

        public async Task<string?> DeleteRecipe(Guid recipeId, Guid userId)
        {
            // Fetch the recipe that belongs to the current user
            var recipe = await context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Favorites)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            if (recipe == null)
                return null; // either doesn't exist or user doesn't own it

            // Remove related data
            context.Ingredients.RemoveRange(recipe.Ingredients);
            context.Steps.RemoveRange(recipe.Steps);
            context.Favorites.RemoveRange(recipe.Favorites);
            context.Ratings.RemoveRange(recipe.Ratings);

            // Remove the recipe itself
            context.Recipes.Remove(recipe); 

            await context.SaveChangesAsync();
            return "Recipe Deleted Successfully.";
        }
    }
}
