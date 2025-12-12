namespace Cmed.Api.Settings;
public record CmedWorkerSettings
{
    public string ConformityOutputDirectoryName {get; set;} = "";
    public string ConformityFileName {get; set;} = "";
    public string ConformitySiteUrl {get; set;} = "";
    public string ConformityOutputDirectoryPath
    {
        get
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var outputDirectoryPath = Path.Combine(currentDirectory, ConformityOutputDirectoryName);

            if (!Directory.Exists(outputDirectoryPath)) Directory.CreateDirectory(outputDirectoryPath);

            return outputDirectoryPath;
        }
    }
    public string ConformityOutputFilePath {
        get
        {
            return Path.Combine(ConformityOutputDirectoryPath, ConformityFileName);
        }
    }  
    public int SleepTimeInMilliseconds = 36000000;
}