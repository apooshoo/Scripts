namespace Scripter.Models.Ui
{
    public class ReseedSelectionOption
    {
        public ReseedSelectionEnum Enum { get; private set; }
        public string Text { get; private set; }

        public ReseedSelectionOption(ReseedSelectionEnum enumm, string text)
        {
            Enum = enumm;
            Text = text;
        }
    }
}
