using UnityEngine;

namespace KidStyo.Helper
{
    public static class TextHelper
    {
        public static void CopyToClipboard(string txt)
        {
            var textEditor = new TextEditor {text = txt};
            textEditor.SelectAll();
            textEditor.Copy();
            Debug.Log($"CopyToClipboard:{txt}");
        }

        public static string PasteFromClipboard()
        {
            var textEditor = new TextEditor {multiline = true};
            textEditor.Paste();
            Debug.Log($"PasteFromClipboard:{textEditor.text}");
            return textEditor.text;
        }
    }
}