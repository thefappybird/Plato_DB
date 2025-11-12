namespace Plato_DB.Interfaces
{
    public class FavoriteFilter : FavoriteFilterBase, IFavoriteFilter
    {
        public Guid UserId { get; set; }
    }

    public class FavoriteFilterBase : IFavoriteFilterBase
    {
        public int Page { get; set; } = 1;
        public string? Search { get; set; } = string.Empty;
    }
}
