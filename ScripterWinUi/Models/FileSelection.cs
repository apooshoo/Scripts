using System;
using System.IO;

namespace ScripterWinUi.Models;

public class FileSelection
{
    public string Name { get; set; }
    public string Extension { get; set; }

    public FileSelection(FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        Name = file.Name;
        Extension = file.Extension;
    }
}