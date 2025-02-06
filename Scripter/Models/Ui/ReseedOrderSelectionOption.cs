﻿namespace Scripter.Models.Ui
{
    public class ReseedOrderSelectionOption
    {
        public ReseedOrderSelectionEnum Enum { get; private set; }
        public string Text { get; private set; }

        public ReseedOrderSelectionOption(ReseedOrderSelectionEnum enumm, string text)
        {
            Enum = enumm;
            Text = text;
        }
    }
}
