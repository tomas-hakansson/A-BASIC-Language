#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix.TerminalMatrixControlFeatures;
using PixelmapLibraryNetFramework;
using PixelmapLibraryNetFramework.FontManagement;
using PixelmapLibraryNetFramework.SpriteManagement;
using TerminalMatrixNetFramework.Definitions;
using TerminalMatrixNetFramework.Events;
using TerminalMatrixNetFramework.TerminalColor;

namespace TerminalMatrixNetFramework;

public partial class TerminalMatrixControl : UserControl
{
    public event TypedLineDelegate? TypedLine;
    public event InputCompletedDelegate? InputCompleted;
    public event UserBreakDelegate? UserBreak;
    public event RequestToggleFullscreenDelegate? RequestToggleFullscreen;
    public event FunctionKeyPressedDelegate? FunctionKeyPressed;
    public event TickDelegate? Tick;

    private DateTime _tickTime;
    private bool[] _terminations;
    private byte[,] _characterColorMap;
    private byte[,] _characterMap;
    private Pixelmap _pixelMap;
    private Bitmap? _fastBitmap;
    private bool _cursorVisibleBlink;
    private readonly System.Windows.Forms.Timer _timer = new();
    private string _lastInput;
    private int _inputBreakVersion;
    private readonly TerminalCodePage _codePage;
    private readonly Palette _palette;
    private TerminalState TerminalState { get; }
    private readonly TerminalMatrixKeypressHandler _keypressHandler;
    private readonly FontMonochromeSprite _fontMonochromeSprite;
    private Size _border;
    private bool _resolutionIncorrect;
    private bool _use32BitForeground;

    public int CharacterWidth { get; private set; } = 40;
    public int CharacterHeight { get; private set; } = 25;
    public int TextRenderLimit { get; private set; }
    private int PixelWidth => CharacterWidth * 8;
    private int PixelHeight => CharacterHeight * 8;

    public RenderingMode RenderingMode { get; set; }
    public Resolution Resolution { get; private set; }
    public bool QuitFlag { get; private set; }
    public Coordinate CursorPosition { get; private set; }
    public byte CurrentCursorColor { get; set; }
    public ProgramLineDictionary ProgramLines { get; } = new();
    public int[,] Background24Bit { get; private set; }
    public bool UseBackground24Bit { get; set; }
    public Action<Graphics>? ControlOverlayPainter { get; set; }
    public bool AutoProgramManagement { get; set; }
    public bool UnlimitedInput { get; set; }

#pragma warning disable CS8618
    public TerminalMatrixControl()
#pragma warning restore CS8618
    {
        _resolutionIncorrect = true;
        _fontMonochromeSprite = FontMonochromeSprite.Create();
        _cursorVisibleBlink = false;
        _lastInput = "";
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        TerminalState = new TerminalState();
        _keypressHandler = new TerminalMatrixKeypressHandler(this);
        CurrentCursorColor = (int)ColorName.White;
        _timer.Interval = 1000;
        QuitFlag = false;
        InitializeComponent();
    }

    /// <summary>
    /// The width of the border in physical pixels.
    /// </summary>
    public int BorderWidth
    {
        get => _border.Width;
        set
        {
            if (_border.Width != value)
                _resolutionIncorrect = true;

            _border.Width = value;
        }
    }

    /// <summary>
    /// The height of the border in physical pixels.
    /// </summary>
    public int BorderHeight
    {
        get => _border.Height;
        set
        {
            if (_border.Height != value)
                _resolutionIncorrect = true;

            _border.Height = value;
        }
    }

    public bool Use32BitForeground
    {
        get => _use32BitForeground;
        set
        {
            if (_use32BitForeground != value)
                _resolutionIncorrect = true;

            _use32BitForeground = value;
        }
    }

    public void SetPixelsToBackground(StillImageSprite image, int x, int y)
    {
        var startX = x + BorderWidth;
        var startY = y + BorderHeight;
        var maxWidth = Background24Bit.GetLength(0) - BorderWidth;
        var maxHeight = Background24Bit.GetLength(1) - BorderHeight;

        for (var sourceY = 0; sourceY < image.Height; sourceY++)
        {
            for (var sourceX = 0; sourceX < image.Width; sourceX++)
            {
                var targetX = startX + sourceX;
                var targetY = startY + sourceY;

                if (targetX >= BorderWidth && targetX < maxWidth && targetY >= BorderHeight && targetY < maxHeight)
                    Background24Bit[targetX, targetY] = image.Get(sourceX, sourceY);
            }
        }
    }

    private void UserControl1_Load(object sender, EventArgs e)
    {
        DoubleBuffered = true;
        SetResolution(Resolution.Pixels320x200Characters40x25);
        _timer.Tick += Blink;
        _timer.Enabled = true;
        _tickTime = DateTime.Now;
    }

    /// <summary>
    /// First visible text line.
    /// </summary>
    /// <param name="limit">0 (all lines are visible) to CharacterHeight - 2 (only the bottom two lines are visible)</param>
    public void SetTextRenderLimit(int limit)
    {
        if (limit < 0 || limit > CharacterHeight - 2)
            throw new ArgumentOutOfRangeException(nameof(limit));

        if (CursorPosition.Y < limit)
            CursorPosition.Y = limit;

        TextRenderLimit = limit;
        UpdateBitmap();
        Invalidate();
    }

    public void SetStartPosition(int x, int y)
    {
        CursorPosition.Set(x, y);
    }

    public void SetResolution(Resolution resolution)
    {
        Resolution = resolution;
        var characterSize = ResolutionHelper.GetCharacterMapSize(resolution);
        CharacterWidth = characterSize.Width;
        CharacterHeight = characterSize.Height;
        TextRenderLimit = Math.Min(TextRenderLimit, CharacterHeight - 2);
        CursorPosition = new Coordinate(0, 0, CharacterWidth, CharacterHeight);
        _characterColorMap = new byte[CharacterWidth, CharacterHeight];
        _characterMap = new byte[CharacterWidth, CharacterHeight];
        _terminations = CharacterMatrixDefinition.CreateTerminations(Resolution);

        _fastBitmap?.Dispose();

        var size = ResolutionHelper.GetPixelSize(resolution, BorderWidth, BorderHeight);
        _fastBitmap = Pixelmap.CreateCompatibleBitmap(size.Width, size.Height);
        _pixelMap = new Pixelmap(_fastBitmap);
        _resolutionIncorrect = false;
        Background24Bit = new int[size.Width, size.Height];

        Clear();
    }

    private void Blink(object? sender, EventArgs e)
    {
        _cursorVisibleBlink = !_cursorVisibleBlink;
        UpdateBitmap();
        Invalidate();

        if (DateTime.Now.Subtract(_tickTime).TotalSeconds >= 0.9)
        {
            _tickTime = DateTime.Now;
            Tick?.Invoke(this, new TickEventArgs(CursorPosition.X, CursorPosition.Y));
        }
    }

    public void ClearColorMap()
    {
        for (var y = 0; y < CharacterHeight; y++)
            for (var x = 0; x < CharacterWidth; x++)
                _characterColorMap[x, y] = CurrentCursorColor;
    }

    public void ClearCharacterMap()
    {
        for (var y = 0; y < CharacterHeight; y++)
            for (var x = 0; x < CharacterWidth; x++)
                _characterMap[x, y] = CharacterMatrixDefinition.CharacterEmpty;

        for (var y = 0; y < CharacterHeight; y++)
            _terminations[y] = true;
    }

    public void ClearPixelMap()
    {
        _pixelMap.LockBits();
        try
        {
            _pixelMap.Clear(Color.Black);
        }
        finally
        {
            _pixelMap.UnlockBits();
        }
    }

    public void Clear()
    {
        ClearColorMap();
        ClearCharacterMap();
        ClearPixelMap();
    }

    public void HorizontalLine(int y, ColorName color)
    {
        for (var x = 0; x < PixelWidth; x++)
            _pixelMap.RangeSafeSetPixel(x, y, _palette.GetColor(color));
    }

    public byte[,] LoadPictureFromGif(string filename)
    {
        using var gif = new Bitmap(filename);
        var result = new byte[gif.Width, gif.Height];

        for (var y = 0; y < gif.Height; y++)
            for (var x = 0; x < gif.Width; x++)
                result[x, y] = _palette.SearchColor(gif.GetPixel(x, y));

        return result;
    }

    public void SetPixel(int x, int y, ColorName color) =>
        _pixelMap.SetPixel(x + BorderWidth, y + BorderHeight, _palette.GetColor(color));

    public void SetPixels(int x, int y, byte[,] colors)
    {
        var targetX = x + BorderWidth;
        var targetY = y + BorderHeight;
        var limitX = PixelWidth + BorderWidth;
        var limitY = PixelHeight + BorderHeight;

        for (var sourceY = 0; sourceY < colors.GetLength(1); sourceY++)
        {
            for (var sourceX = 0; sourceX < colors.GetLength(0); sourceX++)
            {
                if (targetX >= BorderWidth && targetX < limitX && targetY >= BorderHeight && targetY < limitY)
                    _pixelMap.SetPixel(targetX, targetY, _palette.GetColor(colors[sourceX, sourceY]));

                targetX++;
            }

            targetX = x + BorderWidth;
            targetY++;
        }
    }

    public void HorizontalLine(int x1, int y, int x2, ColorName color)
    {
        var c = (byte)color;

        for (var x = x1; x <= x2; x++)
            _pixelMap.SetPixel(x, y, _palette.GetColor(c));
    }

    public void VerticalLine(int x, ColorName color)
    {
        var c = (byte)color;

        for (var y = 0; y < PixelHeight; y++)
            _pixelMap.SetPixel(x, y, _palette.GetColor(c));
    }

    public void VerticalLine(int x, int y1, int y2, ColorName color)
    {
        var c = (byte)color;

        for (var y = y1; y <= y2; y++)
            _pixelMap.SetPixel(x, y, _palette.GetColor(c));
    }

    public void Box(ColorName color, int x1, int y1, int x2, int y2)
    {
        var col = _palette.GetColor((byte)color);

        for (var x = x1; x <= x2; x++)
        {
            _pixelMap.SetPixel(x, y1, col);
            _pixelMap.SetPixel(x, y2, col);
        }

        for (var y = y1 + 1; y < y2; y++)
        {
            _pixelMap.SetPixel(x1, y, col);
            _pixelMap.SetPixel(x2, y, col);
        }
    }

    public void PrintAt(ColorName color, int x, int y, string text)
    {
        var c = (byte)color;

        for (var i = 0; i < text.Length; i++)
        {
            _characterColorMap[x + i, y] = c;
            _characterMap[x + i, y] = _codePage.Asc[text[i]];
        }
    }

    public void UpdateBitmap(Action<Pixelmap>? underText = null, Action<Pixelmap>? overText = null)
    {
        if (QuitFlag)
            return;

        if (_resolutionIncorrect)
        {
            SetResolution(Resolution);
            return;
        }

        _pixelMap.LockBits();
        try
        {
            underText?.Invoke(_pixelMap);

            if (UseBackground24Bit)
            {
                for (var y = 0; y < _pixelMap.Height; y++)
                    for (var x = 0; x < _pixelMap.Width; x++)
                        _pixelMap.SetPixel(x, y, Color.FromArgb(Background24Bit[x, y]));
            }

            for (var y = TextRenderLimit; y < CharacterHeight; y++)
            {
                for (var x = 0; x < CharacterWidth; x++)
                {
                    var pixelX = x * 8 + BorderWidth;
                    var pixelY = y * 8 + BorderHeight;

                    if (CursorPosition.IsSame(x, y))
                    {
                        if (_cursorVisibleBlink)
                            _fontMonochromeSprite.DrawOpaque(_pixelMap, _characterMap[x, y], pixelX, pixelY, _palette.GetColor(CurrentCursorColor), Color.Black);
                        else
                            _fontMonochromeSprite.DrawOpaque(_pixelMap, _characterMap[x, y], pixelX, pixelY, Color.Black, _palette.GetColor(CurrentCursorColor));
                    }
                    else if (_characterMap[x, y] > 0)
                    {
                        _fontMonochromeSprite.DrawOpaque(_pixelMap, _characterMap[x, y], pixelX, pixelY, _palette.GetColor(_characterColorMap[x, y]), Color.Black);
                    }
                }
            }

            overText?.Invoke(_pixelMap);
        }
        finally
        {
            _pixelMap.UnlockBits();
        }
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        Invalidate();
        base.OnResize(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_pixelMap == null!)
            return;

        switch (RenderingMode)
        {
            case RenderingMode.HighSpeed:
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.SmoothingMode = SmoothingMode.None;
                break;
            case RenderingMode.HighQuality:
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        e.Graphics.Clear(Color.Black);

        if (_fastBitmap != null)
            e.Graphics.DrawImage(_fastBitmap, 0, 0, Width, Height);

        ControlOverlayPainter?.Invoke(e.Graphics);

        base.OnPaint(e);
    }

    private void ShowKeyboardActivity()
    {
        _timer.Enabled = false;
        _cursorVisibleBlink = false;
        _timer.Enabled = true;
        UpdateBitmap();
        Invalidate();
    }

    private void TypeCharacter(char c)
    {
        _characterMap[CursorPosition.X, CursorPosition.Y] = (byte)c;
        _characterColorMap[CursorPosition.X, CursorPosition.Y] = CurrentCursorColor;

        if (CursorPosition.X < CharacterWidth - 1)
        {
            CursorPosition.X++;
        }
        else
        {
            // Only an actual wrap joins two physical rows into one logical line.
            var newContinuation = _terminations[CursorPosition.Y];
            _terminations[CursorPosition.Y] = false;
            if (CursorPosition.CanMoveDown())
                CursorPosition.Y++;
            else
                Scroll();

            CursorPosition.X = 0;
            if (newContinuation)
            {
                ClearLine(_characterMap, CursorPosition.Y, CharacterMatrixDefinition.CharacterEmpty);
                ClearLine(_characterColorMap, CursorPosition.Y, CurrentCursorColor);
                _terminations[CursorPosition.Y] = true;
            }
        }

        ShowKeyboardActivity();
    }

    public void WriteText(string text)
    {
        var rows = WordWrapper.WordWrap(text, CharacterWidth);

        foreach (var row in rows)
            WriteLine(row);
    }

    protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Tab:
                e.IsInputKey = true;
                break;
            case Keys.Up:
                e.IsInputKey = true;
                break;
            case Keys.Right:
                e.IsInputKey = true;
                break;
            case Keys.Down:
                e.IsInputKey = true;
                break;
            case Keys.Left:
                e.IsInputKey = true;
                break;
        }

        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Back)
        {
            var limit = TerminalState.InputMode && CursorPosition.Y == TerminalState.InputStartY
                ? TerminalState.InputStartX : 0;

            if (CursorPosition.X > limit)
            {
                CursorPosition.X--;
                DoDelete(_characterMap, _terminations, CharacterMatrixDefinition.CharacterEmpty);
                DoDelete(_characterColorMap, null, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
            }

            return;
        }

        if (e is { KeyCode: Keys.C, Control: true })
        {
            e.SuppressKeyPress = true;
            if (TerminalState.InputMode)
            {
                _inputBreakVersion++;
                TerminalState.InputMode = false;
                _lastInput = "";
            }
            UserBreak?.Invoke(this, e);
            return;
        }

        switch (e.KeyData)
        {
            case Keys.F1:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F1));
                break;
            case Keys.F2:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F2));
                break;
            case Keys.F3:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F3));
                break;
            case Keys.F4:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F4));
                break;
            case Keys.F5:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F5));
                break;
            case Keys.F6:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F6));
                break;
            case Keys.F7:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F7));
                break;
            case Keys.F8:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F8));
                break;
            case Keys.F9:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F9));
                break;
            case Keys.F10:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F10));
                break;
            case Keys.F11:
                RequestToggleFullscreen?.Invoke(this, e);
                break;
            case Keys.PageUp:

                if (TerminalState.InputMode)
                    return;

                CursorPosition.Y = TextRenderLimit;
                ShowEffect();
                break;
            case Keys.PageDown:

                if (TerminalState.InputMode)
                    return;

                CursorPosition.Y = CharacterHeight - 1;
                ShowEffect();
                break;
            case Keys.Home:
                CursorPosition.X = TerminalState.InputMode && CursorPosition.Y == TerminalState.InputStartY
                    ? TerminalState.InputStartX : 0;
                ShowEffect();
                break;
            case Keys.End:
                CursorPosition.X = CharacterWidth - 1;
                ShowEffect();
                break;
            case Keys.Insert:
                DoInsert(_characterMap, _terminations, CharacterMatrixDefinition.CharacterEmpty);
                DoInsert(_characterColorMap, null, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
                break;
            case Keys.Delete:
                DoDelete(_characterMap, _terminations, CharacterMatrixDefinition.CharacterEmpty);
                DoDelete(_characterColorMap, null, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
                break;
            default:
                _keypressHandler.HandleKeyDown(e, TerminalState.InputMode, TerminalState.InputStartX, TerminalState.InputStartY, CursorPosition, ShowKeyboardActivity, Scroll);
                break;
        }

        base.OnKeyDown(e);

        void ShowEffect()
        {
            _timer.Enabled = false;
            _cursorVisibleBlink = true;
            ShowKeyboardActivity();
            _timer.Enabled = true;
        }
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        _keypressHandler.HandleKeyPress(e, TypeCharacter);
        base.OnKeyPress(e);
    }

    internal void HandleEnter(bool shift)
    {
        var firstRow = CursorPosition.Y;
        while (firstRow > 0 && !_terminations[firstRow - 1])
            firstRow--;

        if (TerminalState.InputMode)
            firstRow = Math.Max(firstRow, TerminalState.InputStartY);

        var lastRow = CursorPosition.Y;
        while (lastRow < CharacterHeight - 1 && !_terminations[lastRow])
            lastRow++;

        var inputValue = new StringBuilder();
        for (var y = firstRow; y <= lastRow; y++)
        {
            var start = TerminalState.InputMode && y == TerminalState.InputStartY
                ? TerminalState.InputStartX
                : 0;
            for (var x = start; x < CharacterWidth; x++)
            {
                var c = _characterMap[x, y];
                inputValue.Append(c == 0 ? ' ' : _codePage.Chr[c]);
            }
        }

        // Trim only the completed logical line, preserving spaces at wrap boundaries.
        var value = inputValue.ToString().Trim();
        var programLine = AddProgramLine(value, shift);
        var inputMode = TerminalState.InputMode;
        if (inputMode)
        {
            TerminalState.InputMode = false;
            _lastInput = value;
        }

        _terminations[lastRow] = true;
        CursorPosition.Set(0, lastRow);
        if (CursorPosition.CanMoveDown())
            CursorPosition.Y++;
        else
            Scroll();

        ShowKeyboardActivity();

        // Both completion callbacks observe the cursor on the next line.
        var eventArgs = new TypedLineEventArgs(value);
        if (inputMode)
            InputCompleted?.Invoke(this, eventArgs);
        else if (!programLine && !shift)
            TypedLine?.Invoke(this, eventArgs);
    }

    private void DoInsert(byte[,] map, bool[]? terminations, byte empty) // Accepts the terminations array just in case it is needed.
    {
        if (CursorPosition.X >= CharacterWidth - 1)
        {
            map[CursorPosition.X, CursorPosition.Y] = empty;
            return;
        }

        for (var x = CharacterWidth - 2; x >= CursorPosition.X; x--)
            map[x + 1, CursorPosition.Y] = map[x, CursorPosition.Y];

        map[CursorPosition.X, CursorPosition.Y] = empty;
    }

    private void DoDelete(byte[,] map, bool[]? terminations, byte empty) // Accepts the terminations array just in case it is needed.
    {
        if (CursorPosition.X >= CharacterWidth - 1)
        {
            map[CursorPosition.X, CursorPosition.Y] = empty;
            return;
        }

        for (var x = CursorPosition.X; x < CharacterWidth - 1; x++)
            map[x, CursorPosition.Y] = map[x + 1, CursorPosition.Y];

        map[CharacterWidth - 1, CursorPosition.Y] = empty;
    }

    public SetProgramLinesResult SetProgramLines(string? code)
    {
        var s = (code ?? "").Trim();

        if (string.IsNullOrWhiteSpace(s))
            return SetProgramLinesResult.CreateFail(0, 0, "No data provided.");

        var rows = s.Split("\r\n".ToCharArray());
        var trimmedRows = new List<string>();

        foreach (var row in rows)
        {
            var r = row.Replace("\r", "").Replace("\n", "").Trim();

            if (!string.IsNullOrWhiteSpace(r))
                trimmedRows.Add(r);
        }

        if (trimmedRows.Count <= 0)
            return SetProgramLinesResult.CreateFail(0, 0, "No data provided.");

        ProgramLines.Clear();

        for (var i = 0; i < trimmedRows.Count; i++)
        {
            if (!SetProgramLine(trimmedRows[i]))
                return SetProgramLinesResult.CreateFail(i, trimmedRows.Count, $@"Failed to add: {trimmedRows[i]}");
        }

        return SetProgramLinesResult.CreateSuccess(trimmedRows.Count, trimmedRows.Count);
    }

    public bool SetProgramLine(string row)
    {
        var match = Regex.Match(row, @"^([0-9]+)\s*(.*)$");

        if (match.Success)
        {

            if (!int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var lineNumber))
                return false;

            var line = new ProgramLine(row, lineNumber, match.Groups[2].Value.Trim());

            if (string.IsNullOrWhiteSpace(line.Code))
                ProgramLines.RemoveProgramLine(lineNumber);
            else
                ProgramLines.InsertProgramLine(line);

            return true;
        }

        return false;
    }

    private bool AddProgramLine(string value, bool shift)
    {
        if (shift || TerminalState.InputMode || !AutoProgramManagement)
            return false;

        return SetProgramLine(value);
    }

    private new void Scroll()
    {
        ScrollCharacterMap(_characterColorMap, CurrentCursorColor);
        ScrollCharacterMap(_characterMap, (byte)' ');
        Array.Copy(_terminations, 1, _terminations, 0, _terminations.Length - 1);
        _terminations[_terminations.Length - 1] = true;

        if (!TerminalState.InputMode)
            return;

        TerminalState.InputStartY--;

        if (TerminalState.InputStartY >= 0)
            return;

        TerminalState.InputStartY = 0;
        TerminalState.InputStartX = 0;
    }

    private void ScrollCharacterMap(byte[,] characterMap, byte blank)
    {
        for (var y = 1; y < CharacterHeight; y++)
            for (var x = 0; x < CharacterWidth; x++)
                characterMap[x, y - 1] = characterMap[x, y];

        var last = CharacterHeight - 1;

        for (var x = 0; x < CharacterWidth; x++)
            characterMap[x, last] = blank;
    }

    public void BeginInput(string prompt) =>
        BeginInput(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public void BeginInput(string prompt, string defaultValue) =>
        BeginInput(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public void BeginInput(string prompt, byte promptColor, byte valueColor) =>
        BeginInput(prompt, "", promptColor, valueColor);

    public void BeginInput(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        if (prompt.Length > CharacterWidth - 2)
            prompt = prompt.Substring(0, CharacterWidth - 2);

        ClearLine(_characterMap, CursorPosition.Y, CharacterMatrixDefinition.CharacterEmpty);
        ClearLine(_characterColorMap, CursorPosition.Y, CurrentCursorColor);
        _terminations[CursorPosition.Y] = true;
        
        if (CursorPosition.Y > 0)
            _terminations[CursorPosition.Y - 1] = true;
        
        CurrentCursorColor = promptColor;
        Write(prompt);
        CurrentCursorColor = valueColor;
        TerminalState.InputStartX = prompt.Length;
        TerminalState.InputStartY = CursorPosition.Y;
        CursorPosition.X = TerminalState.InputStartX;
        TerminalState.InputMode = true;
        
        foreach (var c in defaultValue)
            TypeCharacter(c);
        
        UpdateBitmap();
        Invalidate();
    }

    private void ClearLine(byte[,] map, int y, byte clear)
    {
        for (var x = 0; x < CharacterWidth; x++)
            map[x, y] = clear;
    }

    public void List() =>
        DisplayLargeListStrategy.List(WriteLine, Refresh, ProgramLines);

    public bool GetTerminator(int y)
    {
        if (y < 0 || y >= CharacterHeight)
            return false;

        return _terminations[y];
    }

    public void SetTerminator(int y, bool terminator)
    {
        if (y < 0 || y >= CharacterHeight)
            return;

        _terminations[y] = terminator;
    }

    public void Write(string text) =>
        Write(0, text);

    public void Write(int xStart, string text)
    {
        var y = CursorPosition.Y;

        for (var x = 0; x < text.Length; x++)
        {
            if (x + xStart >= CharacterWidth)
                break;
            _characterMap[x + xStart, y] = (byte)text[x];
            _characterColorMap[x + xStart, y] = CurrentCursorColor;
        }

        UpdateBitmap();
        Invalidate();
    }

    public void WriteLine(string text)
    {
        var y = CursorPosition.Y;

        for (var x = 0; x < text.Length; x++)
        {
            if (x >= CharacterWidth)
                break;

            _characterMap[x, y] = (byte)text[x];
            _characterColorMap[x, y] = CurrentCursorColor;
        }

        CursorPosition.X = 0;
        _terminations[CursorPosition.Y] = true;

        if (CursorPosition.CanMoveDown())
            CursorPosition.Y++;
        else
            Scroll();

        UpdateBitmap();
        Invalidate();
    }

    public string InputString(string prompt) =>
        InputString(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public string InputString(string prompt, string defaultValue) =>
        InputString(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public string InputString(string prompt, byte promptColor, byte valueColor) =>
        InputString(prompt, "", promptColor, valueColor);

    public string InputString(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        var breakVersion = _inputBreakVersion;
        BeginInput(prompt, defaultValue, promptColor, valueColor);

        do
        {
            if (QuitFlag)
                return "";

            Thread.Yield();
            Thread.Sleep(2);
            Application.DoEvents();
        } while (TerminalState.InputMode && !QuitFlag && breakVersion == _inputBreakVersion);

        if (QuitFlag || breakVersion != _inputBreakVersion)
        {
            TerminalState.InputMode = false;
            return "";
        }

        return _lastInput;
    }

    public double InputDouble(string prompt) =>
        InputDouble(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public double InputDouble(string prompt, string defaultValue) =>
        InputDouble(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public double InputDouble(string prompt, byte promptColor, byte valueColor) =>
        InputDouble(prompt, "", promptColor, valueColor);

    public double InputDouble(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        do
        {
            var breakVersion = _inputBreakVersion;
            var inputResult = InputString(prompt, defaultValue, promptColor, valueColor);
            if (QuitFlag || breakVersion != _inputBreakVersion)
                return 0;

            if (double.TryParse(inputResult, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            WriteLine("?Expected float, redo from start");
        } while (true);
    }

    public int InputInteger(string prompt) =>
        InputInteger(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public int InputInteger(string prompt, string defaultValue) =>
        InputInteger(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public int InputInteger(string prompt, byte promptColor, byte valueColor) =>
        InputInteger(prompt, "", promptColor, valueColor);

    public int InputInteger(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        do
        {
            var breakVersion = _inputBreakVersion;
            var inputResult = InputString(prompt, defaultValue, promptColor, valueColor);
            if (QuitFlag || breakVersion != _inputBreakVersion)
                return 0;

            if (int.TryParse(inputResult, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            WriteLine("?Expected integer, redo from start");
        } while (true);
    }

    public string GetProgramAsString()
    {
        if (ProgramLines.Count <= 0)
            return "";

        var s = new StringBuilder();

        foreach (var line in ProgramLines)
        {
            s.AppendLine($"{line.Key} {line.Value.Code}");
        }

        return s.ToString();
    }

    public void New() =>
        ProgramLines.Clear();

    public void Quit() =>
        QuitFlag = true;
}
