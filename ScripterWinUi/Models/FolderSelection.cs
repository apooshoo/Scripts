using System;

namespace ScripterWinUi.Models;

public class FolderSelection
{
    public string Name { get; set; }

    public FolderSelection(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }
}