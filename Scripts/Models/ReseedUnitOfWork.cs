namespace Scripts.Models
{
    public class ReseedUnitOfWork
    {
        public FileInfo File { get; private set; }
        public string Destination { get; private set; }

        // Use a temp file path for staging to avoid clashes (eg: 1.jpg -> 2.jpg while 2.jpg exists)
        public string Staging { get; private set; }

        public ReseedUnitOfWork(FileInfo file, int i)
        {
            File = file;
            Destination = Path.Combine(file.DirectoryName, i + file.Extension);
            Staging = Path.Combine(file.DirectoryName, Path.GetRandomFileName() + file.Extension);
        }

        public void MoveToStaging()
        {
            File.MoveTo(Staging);
        }

        public void MoveToDestination()
        {
            File.MoveTo(Destination);
        }
    }
}
