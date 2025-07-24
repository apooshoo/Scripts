using System;

namespace ScripterWinUi.Models.Ui;

public class FolderSelectionOption
{
    public FolderSelectionEnum Enum { get; private set; }
    public string Text { get; private set; }

    public FolderSelectionOption(FolderSelectionEnum enumm, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Enum = enumm;
        Text = text;
    }

    // Override ToString for ComboBox display
    public override string ToString()
    {
        return Text;
    }
}