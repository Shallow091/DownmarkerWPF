﻿using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit;
using MarkPad.Framework.Events;

namespace MarkPad.Document.EditorBehaviours
{
    public class AutoContinueLists : IHandle<EditorPreviewKeyDownEvent>
    {
        // [\s]*    zero or more whitespace chars
        // [-\*]    one of '-' or '*'
        // [\s]+    one or more whitespace chars
        readonly Regex _unorderedListRegex = new Regex(@"[\s]*[-\*][\s]+", RegexOptions.Compiled);

        public void Handle(EditorPreviewKeyDownEvent e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            if (e.Args.Key != Key.Enter) return;

            if (!e.Editor.IsCaratAtEndOfLine()) return;

            var handled = false;

            handled = handled || HandleUnorderedList(e.Editor);

            e.Args.Handled = handled;
        }

        bool HandleUnorderedList(TextEditor editor)
        {
            var match = _unorderedListRegex.Match(editor.GetTextLeftOfCursor());

            if (!match.Success) return false;

            if (match.Value == editor.GetTextLeftOfCursor())
            {
                var currentPosition = editor.TextArea.Caret.Offset;
                editor.SelectionStart = editor.GetCurrentLine().Offset;
                editor.SelectionLength = currentPosition - editor.GetCurrentLine().Offset;
                editor.TextArea.Selection.ReplaceSelectionWithText("");
            }
            else
            {
                editor.TextArea.Selection.ReplaceSelectionWithText(Environment.NewLine + match.Value);
            }

            return true;
        }
    }
}
