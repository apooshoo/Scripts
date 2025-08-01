using ScripterWinUi.Models.Ui;

namespace ScripterWinUi.Services;

public static class UiSelectionOptions
{
    public static readonly FolderSelectionOption[] DefaultFolderSelectionOptions =
    [
        new FolderSelectionOption(FolderSelectionEnum.Folder, "Selected folder"),
        new FolderSelectionOption(FolderSelectionEnum.SubFolders, "Subfolders of selected folder")
    ];

    public static readonly ReseedOrderSelectionOption[] DefaultReseedSelectionOptions =
    [
        new ReseedOrderSelectionOption(ReseedOrderSelectionEnum.FileName, "File name"),
        new ReseedOrderSelectionOption(ReseedOrderSelectionEnum.CreationDate, "Creation Date")
    ];
}