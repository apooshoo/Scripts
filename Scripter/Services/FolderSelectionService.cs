using Scripter.Models;

namespace Scripter.Services
{
    public static class FolderSelectionService
    {
        public static readonly FolderSelectionOption[] DefaultOptions =
        {
            new FolderSelectionOption(FolderSelectionEnum.Folder, "Selected folder"),
            new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders of selected folder")
        };
    }
}
