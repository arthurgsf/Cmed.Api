using Cmed.Api.Settings;
using Microsoft.Extensions.Options;

namespace Cmed.Api.Services;

public class ConformityService(
    IOptions<CmedWorkerSettings> cmedWorkerSettings
): IConformityService
{
    private readonly IOptions<CmedWorkerSettings> _cmedWorkerSettings = cmedWorkerSettings;
    public bool GetIsUpdated(DateTimeOffset queryDateTimeOffset)
    {
        if(!File.Exists(_cmedWorkerSettings.Value.ConformityOutputFilePath)) return true;

        var fileCreationTimeLocalDateTime = File.GetCreationTime(_cmedWorkerSettings.Value.ConformityOutputFilePath);
        var fileCreationTime = new DateTimeOffset(fileCreationTimeLocalDateTime);

        return queryDateTimeOffset.CompareTo(fileCreationTime) > 0;
    }

    public Stream GetLatestFile()
    {
        string path = Path.Combine(_cmedWorkerSettings.Value.ConformityOutputFilePath);
        if (!File.Exists(path)) throw new FileNotFoundException();
        return File.OpenRead(path);
    }
}