namespace Plato_DB.Dtos.User.UserToken
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }

}
