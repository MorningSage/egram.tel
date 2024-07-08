namespace Tel.Egram.Services.Persistence;

public interface IFileExplorer
{
    /// <summary>
    /// Open directory in explorer, finder, etc.
    /// </summary>
    void OpenDirectory(DirectoryInfo directory);
        
    /// <summary>
    /// Open directory in explorer, finder, etc. cotaining the file
    /// </summary>
    void OpenDirectory(FileInfo file);
}