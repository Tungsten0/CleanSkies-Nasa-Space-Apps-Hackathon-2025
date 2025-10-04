namespace CleanSkies.Services
{
    public interface IOpenAQApi
    {
        Task<string> GetLocationDataAsync(int locationId);
    }
}