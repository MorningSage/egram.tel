using System.Diagnostics;
using Tel.Egram.Services.Utils.Platforms;

namespace Tel.Egram.Services.Persistence;

public class FileExplorer(IPlatform platform) : IFileExplorer
{
    /// <summary>
    /// <inheritdoc cref="IFileExplorer"/>
    /// </summary>
    public void OpenDirectory(DirectoryInfo directory)
    {
        switch (platform)
        {
            case WindowsPlatform _:
                Process.Start("explorer.exe", $"\"{directory.FullName}\"");
                break;
                
            case MacosPlatform _:
                Process.Start("open", $"\"{directory.FullName}\"");
                break;
            
            case LinuxPlatform _:
                // ToDo: Add linux support for opening a folder?
                break;
            
        }
    }

    /// <summary>
    /// <inheritdoc cref="IFileExplorer"/>
    /// </summary>
    public void OpenDirectory(FileInfo file)
    {
        switch (platform)
        {
            case WindowsPlatform _:
                Process.Start("explorer.exe", $"/select,\"{file.FullName}\"");
                break;

            case MacosPlatform _:
                Process.Start("open", $"-R \"{file.FullName}\"");
                break;
        }
    }
}