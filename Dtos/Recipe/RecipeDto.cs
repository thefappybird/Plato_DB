using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plato_DB.Dtos.Recipe
{
    public class RecipeDto
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AuthorUsername { get; set; } = string.Empty;

        public double AverageRating { get; set; }
        public int TotalFavorites { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; } = false;
    }
    public class GetRecipeDto
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AuthorUsername { get; set; } = string.Empty;

        public List<IngredientDto> Ingredients { get; set; } = new();
        public List<StepDto> Steps { get; set; } = new();
        public double AverageRating { get; set; }
        public int TotalFavorites { get; set; }
        public bool IsFavoritedByCurrentUser { get; set; } = false;
    }
    public class CreateRecipeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }

        [FromForm(Name = "ingredients")]
        public string IngredientsJson { get; set; } = string.Empty;

        [FromForm(Name = "steps")]
        public string StepsJson { get; set; } = string.Empty;

        [NotMapped]
        public List<IngredientDto> Ingredients =>
            string.IsNullOrEmpty(IngredientsJson)
                ? new()
                : JsonConvert.DeserializeObject<List<IngredientDto>>(IngredientsJson)!;

        [NotMapped]
        public List<StepDto> Steps =>
            string.IsNullOrEmpty(StepsJson)
                ? new()
                : JsonConvert.DeserializeObject<List<StepDto>>(StepsJson)!;
    }
    public class UpdateRecipeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }

        [FromForm(Name = "ingredients")]
        public string IngredientsJson { get; set; } = string.Empty;

        [FromForm(Name = "steps")]
        public string StepsJson { get; set; } = string.Empty;

        [NotMapped]
        public List<IngredientDto> Ingredients =>
            string.IsNullOrEmpty(IngredientsJson)
                ? new()
                : JsonConvert.DeserializeObject<List<IngredientDto>>(IngredientsJson)!;

        [NotMapped]
        public List<StepDto> Steps =>
            string.IsNullOrEmpty(StepsJson)
                ? new()
                : JsonConvert.DeserializeObject<List<StepDto>>(StepsJson)!;
    }
    public class IngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Units { get; set; } = string.Empty;
    }

    public class StepDto
    {
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
    }

}

