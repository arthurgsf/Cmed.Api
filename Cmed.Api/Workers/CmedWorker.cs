using Cmed.Api.Settings;
using Cmed.Scrapper;
using Microsoft.Extensions.Options;

namespace Cmed.Api.Workers;

public class CmedWorker(
    ICmedScrapper cmedScrapper,
    ILogger<CmedWorker> logger,
    IOptions<CmedWorkerSettings> settings
): BackgroundService
{
    private readonly IOptions<CmedWorkerSettings> _settings = settings;
    private readonly ILogger _logger = logger;
    
    private readonly ICmedScrapper _cmedScrapper = cmedScrapper;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tempFilePath = Path.Combine(_settings.Value.ConformityOutputDirectoryPath, "temp.csv");
        var outputFilePath = _settings.Value.ConformityOutputFilePath;

        _logger.LogDebug(
            "Initializing Cmed Worker\nOutput File: {output_file}\nSleep Time: {sleep_time}",
            outputFilePath,
            _settings.Value.SleepTimeInMilliseconds
        );
        var latestUrl = "";
        while (!stoppingToken.IsCancellationRequested)
        {
            
            var currentUrl = await _cmedScrapper.GetLatestFileUrlAsync();
            if (latestUrl != currentUrl)
            {
                latestUrl = currentUrl;
                
                _logger.LogDebug("Downloading from {url}", currentUrl);
                var csv = await _cmedScrapper.GetCsvFromUrlAsync(currentUrl);

                await File.WriteAllTextAsync(tempFilePath, csv, stoppingToken);
                if (File.Exists(outputFilePath))
                {
                    File.Replace(tempFilePath, outputFilePath, null);
                }
                else
                {
                    File.Move(tempFilePath, outputFilePath);
                }

                _logger.LogInformation("Download finished {finished_date}, csv written to {output_file_path}", DateTimeOffset.Now, outputFilePath);
            }
            await Task.Delay(_settings.Value.SleepTimeInMilliseconds, stoppingToken);
        }
    }
}

//TODO : add proper error treatment