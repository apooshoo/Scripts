namespace Scripter.Models
{
    public class FolderSelectionOption
    {
        public FolderSelectionEnum Enum { get; private set; }
        public string Text { get; private set; }

        public FolderSelectionOption(FolderSelectionEnum enumm, string text)
        {
            Enum = enumm;
            Text = text;
        }
    }
}
