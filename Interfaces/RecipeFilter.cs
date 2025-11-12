namespace Plato_DB.Interfaces
{
    public class RecipeFilter : IRecipeFilter
    {
        public string? title { get; set; }
        public string? authorName { get; set; }
        public int page { get; set; } = 1;
        public string? sort { get; set; }
    }
}
