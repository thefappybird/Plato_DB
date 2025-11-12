namespace Plato_DB.Interfaces
{
    public class RegisterResponse : IRegisterResponse
    {
        public bool Passed { get; set; }
        public string? Message { get; set; }
    }
}
