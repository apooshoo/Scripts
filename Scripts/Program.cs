using Microsoft.VisualBasic.FileIO;

var scripter = new ScriptRunner();
scripter.Run();

public class ScriptRunner
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




