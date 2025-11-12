namespace Plato_DB.Models
{
    public class Recipe
    {
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? ImageUrl { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<Step> Steps { get; set; } = new List<Step>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
    public class Ingredient
    {
        public Guid IngredientId { get; set; }
        public Guid RecipeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; } = 1;
        public string Units { get; set; } = string.Empty;
        public Recipe Recipe { get; set; } = null!;
    }
    public class Step
    {
        public Guid StepId { get; set; }
        public Guid RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public Recipe Recipe { get; set; } = null!;
    }
}

