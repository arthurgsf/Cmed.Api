using System.Text;
using Cmed.Api.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Cmed.Api.Services;

public class ConformityService(
    IOptions<CmedWorkerSettings> cmedWorkerSettings,
    IMemoryCache cache
): IConformityService
{
    private readonly IMemoryCache _cache = cache;
    private readonly IOptions<CmedWorkerSettings> _cmedWorkerSettings = cmedWorkerSettings;
    public bool GetIsUpdated(DateTimeOffset queryDateTimeOffset)
    {
        if(!File.Exists(_cmedWorkerSettings.Value.ConformityOutputFilePath)) return true;

        var fileCreationTimeLocalDateTime = File.GetCreationTime(_cmedWorkerSettings.Value.ConformityOutputFilePath);
        var fileCreationTime = new DateTimeOffset(fileCreationTimeLocalDateTime);

        return queryDateTimeOffset.CompareTo(fileCreationTime) > 0;
    }

    public async Task<Stream> GetLatestFileAsync()
    {
        string path = Path.Combine(_cmedWorkerSettings.Value.ConformityOutputFilePath);
        var csvBytes = _cache.Get<byte[]>(_cmedWorkerSettings.Value.ConformityFileName);
        if (csvBytes?.Length > 0) // cache hit
        {
            var stream = new MemoryStream(csvBytes);
            return stream;
        }

        // cache miss
        if (!File.Exists(path)) throw new FileNotFoundException();
        return File.OpenRead(path);
    }
}