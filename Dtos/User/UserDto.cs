using Plato_DB.Dtos.Other;
using Plato_DB.Dtos.Recipe;

namespace Plato_DB.Dtos.User
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public ICollection<RecipeDto> Recipes { get; set; } = new List<RecipeDto>();
        public ICollection<FavoriteDto> Favorites { get; set; } = new List<FavoriteDto>();
        public ICollection<RatingDto> Ratings { get; set; } = new List<RatingDto>();
    }
}
