namespace Plato_DB.Interfaces
{
    public interface IFavoriteFilterBase
    {
        int Page { get; set; }
        string? Search { get; set; }
    }
    public interface IFavoriteFilter : IFavoriteFilterBase
    {
        Guid UserId { get; set; }

    }
}
