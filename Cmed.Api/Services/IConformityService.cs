namespace Cmed.Api.Services;

public interface IConformityService
{
    public Task<Stream> GetLatestFileAsync();
    public bool GetIsUpdated(DateTimeOffset queryDateTimeOffset);
}