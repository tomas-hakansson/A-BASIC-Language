using System.IO;
using A_BASIC_Language.TerminalMatrix;

namespace A_BASIC_Language.Language;

/// <summary>Text operations needed by BASIC; Write appends at the current output position.</summary>
public interface IBasicTerminal
{
    bool QuitFlag { get; }
    int OutputColumn { get; }
    Resolution ScreenResolution { get; }
    void SetScreenResolution(Resolution resolution, bool preserveContents = false);
    void Write(string text);
    void WriteCharacterAt(int x, int y, char character);
    void CreateBlockSprite(int number, int x, int y, char character);
    void MoveBlockSprite(int number, int x, int y, char? character);
    void MoveBlockSpriteRelative(int number, int relativeX, int relativeY);
    void DeleteBlockSprite(int? number);
    void WriteLine(string text);
    void WriteLine(bool blankLineFirst, string text);
    string InputString(string prompt);
    void PumpEvents();
    void ClearScreen();
    void SetTextRenderLimit(int limit);
    void SetTextColor(int color);
    void SetBackgroundColor(int color);
    void SetBorderColor(int color);
    void MainWindowLoadProgram(string source);
    bool MainWindowSaveProgram(string source);
    void MainWindowSessionSetFolders(FileInfo basicFileInfo);
}
