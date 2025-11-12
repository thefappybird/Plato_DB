using Plato_DB.Dtos.Recipe;

namespace Plato_DB.Dtos.Other
{
    public class FavoriteDto
    {
        public Guid FavoriteId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string AuthorUsername { get; set; } = string.Empty;
        public RecipeDto Recipe { get; set; } = new();
    }
}
