namespace Plato_DB.Models
{
    public class Favorite
    {
        public Guid FavoriteId { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
    }
}
