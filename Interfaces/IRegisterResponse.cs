namespace Plato_DB.Interfaces
{
    public interface IRegisterResponse
    {
        bool Passed { get; set; }
        string? Message { get; set; }
    }
}
