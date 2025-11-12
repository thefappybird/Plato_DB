namespace Plato_DB.Dtos.Other
{
    public class RatingDto
    {
        public Guid RatingId { get; set; }
        public int Score { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string AuthorUsername { get; set; } = string.Empty;
        public string RecipeTitle { get; set; } = string.Empty;
    }
    public class CreateRatingDto
    {
        public int Score { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
    }
}
