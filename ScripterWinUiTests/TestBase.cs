using System.Reflection;

namespace ScripterWinUiTests;

public abstract class TestBase
{
    public string TestBaseFolder
    {
        get
        {
            return Path.Combine(GetRootDir(), "TestFolder");
        }
    }

    public string TestOutputFolder
    {
        get
        {
            var outputFolder = Path.Combine(TestBaseFolder, "Output");
            if (Directory.Exists(outputFolder))
            {
                return outputFolder;
            }
            else
            {
                Directory.CreateDirectory(outputFolder);
                return outputFolder;
            }
        }
    }

    public string GetTestFolder(string folderName) => Path.Combine(TestBaseFolder, folderName);

    private string GetRootDir() =>
        new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName ?? throw new Exception();

    public static void CleanUpFolder(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void CopyFolderContents(string from, string to)
    {
        var dir = new DirectoryInfo(from);
        foreach (var file in dir.GetFiles())
        {
            file.CopyTo(Path.Combine(to, file.Name));
        }
    }

    public static void CreateEmptyFile(string fileName)
    {
        File.Create(fileName).Dispose();
    }

    public static void CreateTestSubFolders(string basePath, params string[] folderNames)
    {
        foreach (var folderName in folderNames)
        {
            Directory.CreateDirectory(Path.Combine(basePath, folderName));
        }
    }
}