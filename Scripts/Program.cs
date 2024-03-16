using Microsoft.VisualBasic.FileIO;
using Scripts;
using System.Diagnostics;
using System.Text.RegularExpressions;

var scripter = new Scripter();
scripter.Run();

public class Scripter
{
    public void Run()
    {
        var path = @"";

        //RunForSubFolders(path);
        //RunForFolder(path);
    }


    private void RunForSubFolders(string path)
    {
        var folderPath = Directory.GetDirectories(path);
        foreach (var subFolderPath in folderPath)
        {
            RunForFolder(subFolderPath);
        }
    }


    private void RunForFolder(string path)
    {
        //Notify(path);

        //FileRenamer.NormaliseFilenames(path);
        //FileRenamer.KeepFirstXAndLastYCharacters(path, 0, 3);

        //FileConverter.ConvertWebps(path);
    }

    private void Notify(string path)
    {
        var folders = FileSystem.GetDirectories(path);
        if (folders.Count == 1) 
        {
            Console.WriteLine("Single subfolder found in: " + folders.First().ToLower());
        }
    }
}




