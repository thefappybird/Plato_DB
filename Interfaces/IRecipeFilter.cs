namespace Plato_DB.Interfaces
{
    public interface IRecipeFilter
    {
        string? title { get; set; }
        string? authorName { get; set; }
        int page { get; set; }
        string? sort { get; set; }
    }
}
