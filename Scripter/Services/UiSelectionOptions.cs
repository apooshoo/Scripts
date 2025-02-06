using Scripter.Models.Ui;

namespace Scripter.Services
{
    public static class UiSelectionOptions
    {
        public static readonly FolderSelectionOption[] DefaultFolderSelectionOptions =
        [
            new FolderSelectionOption(FolderSelectionEnum.Folder, "Selected folder"),
            new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders of selected folder")
        ];

        public static readonly ReseedSelectionOption[] DefaultReseedSelectionOptions =
        [
            new ReseedSelectionOption(ReseedSelectionEnum.FileName, "File name"),
            new ReseedSelectionOption(ReseedSelectionEnum.ModifiedDate, "Modified Date")
        ];
    }
}
