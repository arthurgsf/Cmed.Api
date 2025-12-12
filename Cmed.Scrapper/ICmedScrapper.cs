namespace Cmed.Scrapper;

public interface ICmedScrapper
{
    // has changed
    Task<string> GetLatestFileUrlAsync();
    Task<string> GetCsvFromUrlAsync(string fileUrl);
}