namespace Cmed.Api.Services;

public interface IConformityService
{
    public Stream GetLatestFile();
    public bool GetIsUpdated(DateTimeOffset queryDateTimeOffset);
}