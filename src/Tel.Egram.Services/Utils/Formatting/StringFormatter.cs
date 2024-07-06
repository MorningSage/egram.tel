namespace Tel.Egram.Services.Utils.Formatting;

public class StringFormatter : IStringFormatter
{
    private static readonly string[] FileSizes = [ "B", "KB", "MB", "GB", "TB" ];
    
    public string FormatShortTime(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("hh:mm");
    }

    public string FormatShortTime(int timestamp)
    {
        var time = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
        return FormatShortTime(time);
    }

    public string FormatMemorySize(long bytes)
    {
        var order = 0;
        
        while (bytes >= 1024 && order < FileSizes.Length - 1) {
            order++;
            bytes /= 1024;
        }
        
        return $"{bytes:0.##} {FileSizes[order]}";
    }
}