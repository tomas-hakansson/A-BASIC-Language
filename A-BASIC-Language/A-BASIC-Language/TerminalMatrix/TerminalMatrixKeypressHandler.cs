using System;
using System.Windows.Forms;
using PixelmapLibraryNetFramework.FontManagement;

namespace TerminalMatrixNetFramework;

internal class TerminalMatrixKeypressHandler
{
    private readonly TerminalMatrixControl _owner;
    
    internal TerminalMatrixKeypressHandler(TerminalMatrixControl owner)
    {
        _owner = owner;
    }

    internal void HandleKeyPress(KeyPressEventArgs e, Action<char> typeCharacter)
    {
        if (TerminalCodePage.Characters.IndexOf(e.KeyChar) >= 0)
            typeCharacter(e.KeyChar);
    }

    internal void HandleKeyDown(KeyEventArgs e, bool inputMode, int inputStartX, int inputStartY, Coordinate cursorPosition, Action showKeyboardActivity, Action scroll)
    {
        if (e.KeyCode == Keys.Enter)
        {
            _owner.HandleEnter((e.Modifiers & Keys.Shift) != 0);
            return;
        }

        switch (e.KeyData)
        {
            case Keys.Left:
                if (inputMode)
                {
                    var limit = cursorPosition.Y == inputStartY ? inputStartX : 0;
                    if (cursorPosition.X > limit)
                    {
                        cursorPosition.X--;
                        showKeyboardActivity();
                    }
                    else if (cursorPosition.Y > inputStartY)
                    {
                        cursorPosition.X = _owner.CharacterWidth - 1;
                        cursorPosition.Y--;
                        showKeyboardActivity();
                    }
                }
                else
                {
                    if (cursorPosition.X > 0)
                    {
                        cursorPosition.X--;
                        showKeyboardActivity();
                    }
                    else if (cursorPosition.Y > _owner.TextRenderLimit)
                    {
                        cursorPosition.X = _owner.CharacterWidth - 1;
                        cursorPosition.Y--;
                        showKeyboardActivity();
                    }
                }
                break;
            case Keys.Up:
                if (inputMode)
                    return;

                if (cursorPosition.Y > _owner.TextRenderLimit)
                {
                    cursorPosition.Y--;
                    showKeyboardActivity();
                }
                break;
            case Keys.Right:
                if (inputMode)
                {
                    if (cursorPosition.X < _owner.CharacterWidth - 1)
                    {
                        cursorPosition.X++;
                        showKeyboardActivity();
                    }
                    else if (!_owner.GetTerminator(cursorPosition.Y) && cursorPosition.Y < _owner.CharacterHeight - 1)
                    {
                        cursorPosition.X = 0;
                        cursorPosition.Y++;
                        showKeyboardActivity();
                    }
                }
                else
                {
                    if (cursorPosition.X < _owner.CharacterWidth - 1)
                    {
                        cursorPosition.X++;
                        showKeyboardActivity();
                    }
                    else if (cursorPosition.Y < _owner.CharacterHeight - 1)
                    {
                        cursorPosition.X = 0;
                        cursorPosition.Y++;
                        showKeyboardActivity();
                    }
                }
                break;
            case Keys.Down:
                if (inputMode)
                    return;

                if (cursorPosition.Y < _owner.CharacterHeight - 1)
                {
                    cursorPosition.Y++;
                    showKeyboardActivity();
                }
                else
                {
                    scroll();
                    showKeyboardActivity();
                }
                break;
        }
    }
}
