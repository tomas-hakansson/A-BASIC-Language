namespace A_BASIC_Language.Gui;

public class ColorPalette : IDisposable
{
    public Color Color01Black { get; }
    public Color Color02White { get; }
    public Color Color03Red { get; }
    public Color Color04Cyan { get; }
    public Color Color05Purple { get; }
    public Color Color06Green { get; }
    public Color Color07Blue { get; }
    public Color Color08Yellow { get; }
    public Color Color09Orange { get; }
    public Color Color10Brown { get; }
    public Color Color11Pink { get; }
    public Color Color12DarkGrey { get; }
    public Color Color13Grey { get; }
    public Color Color14LightGreen { get; }
    public Color Color15LightBlue { get; }
    public Color Color16LightGrey { get; }

    public Pen Pen01Black { get; }
    public Pen Pen02White { get; }
    public Pen Pen03Red { get; }
    public Pen Pen04Cyan { get; }
    public Pen Pen05Purple { get; }
    public Pen Pen06Green { get; }
    public Pen Pen07Blue { get; }
    public Pen Pen08Yellow { get; }
    public Pen Pen09Orange { get; }
    public Pen Pen10Brown { get; }
    public Pen Pen11Pink { get; }
    public Pen Pen12DarkGrey { get; }
    public Pen Pen13Grey { get; }
    public Pen Pen14LightGreen { get; }
    public Pen Pen15LightBlue { get; }
    public Pen Pen16LightGrey { get; }

    public Brush Brush01Black { get; }
    public Brush Brush02White { get; }
    public Brush Brush03Red { get; }
    public Brush Brush04Cyan { get; }
    public Brush Brush05Purple { get; }
    public Brush Brush06Green { get; }
    public Brush Brush07Blue { get; }
    public Brush Brush08Yellow { get; }
    public Brush Brush09Orange { get; }
    public Brush Brush10Brown { get; }
    public Brush Brush11Pink { get; }
    public Brush Brush12DarkGrey { get; }
    public Brush Brush13Grey { get; }
    public Brush Brush14LightGreen { get; }
    public Brush Brush15LightBlue { get; }
    public Brush Brush16LightGrey { get; }

    public ColorPalette()
    {
        Color01Black = Color.FromArgb(0, 0, 0);
        Color02White = Color.FromArgb(255, 255, 255);
        Color03Red = Color.FromArgb(136, 0, 0);
        Color04Cyan = Color.FromArgb(170, 255, 238);
        Color05Purple = Color.FromArgb(204, 68, 204);
        Color06Green = Color.FromArgb(0, 204, 85);
        Color07Blue = Color.FromArgb(0, 0, 170);
        Color08Yellow = Color.FromArgb(238, 238, 119);
        Color09Orange = Color.FromArgb(221, 136, 85);
        Color10Brown = Color.FromArgb(102, 68, 0);
        Color11Pink = Color.FromArgb(255, 119, 119);
        Color12DarkGrey = Color.FromArgb(51, 51, 51);
        Color13Grey = Color.FromArgb(119, 119, 119);
        Color14LightGreen = Color.FromArgb(170, 255, 102);
        Color15LightBlue = Color.FromArgb(0, 136, 255);
        Color16LightGrey = Color.FromArgb(187, 187, 187);

        Pen01Black = new Pen(Color01Black);
        Pen02White = new Pen(Color02White);
        Pen03Red = new Pen(Color03Red);
        Pen04Cyan = new Pen(Color04Cyan);
        Pen05Purple = new Pen(Color05Purple);
        Pen06Green = new Pen(Color06Green);
        Pen07Blue = new Pen(Color07Blue);
        Pen08Yellow = new Pen(Color08Yellow);
        Pen09Orange = new Pen(Color09Orange);
        Pen10Brown = new Pen(Color10Brown);
        Pen11Pink = new Pen(Color11Pink);
        Pen12DarkGrey = new Pen(Color12DarkGrey);
        Pen13Grey = new Pen(Color13Grey);
        Pen14LightGreen = new Pen(Color14LightGreen);
        Pen15LightBlue = new Pen(Color15LightBlue);
        Pen16LightGrey = new Pen(Color16LightGrey);

        Brush01Black = new SolidBrush(Color01Black);
        Brush02White = new SolidBrush(Color02White);
        Brush03Red = new SolidBrush(Color03Red);
        Brush04Cyan = new SolidBrush(Color04Cyan);
        Brush05Purple = new SolidBrush(Color05Purple);
        Brush06Green = new SolidBrush(Color06Green);
        Brush07Blue = new SolidBrush(Color07Blue);
        Brush08Yellow = new SolidBrush(Color08Yellow);
        Brush09Orange = new SolidBrush(Color09Orange);
        Brush10Brown = new SolidBrush(Color10Brown);
        Brush11Pink = new SolidBrush(Color11Pink);
        Brush12DarkGrey = new SolidBrush(Color12DarkGrey);
        Brush13Grey = new SolidBrush(Color13Grey);
        Brush14LightGreen = new SolidBrush(Color14LightGreen);
        Brush15LightBlue = new SolidBrush(Color15LightBlue);
        Brush16LightGrey = new SolidBrush(Color16LightGrey);
    }

    public Pen GetPen(int index) =>
        index switch
        {
            0 => Pen01Black,
            1 => Pen02White,
            2 => Pen03Red,
            3 => Pen04Cyan,
            4 => Pen05Purple,
            5 => Pen06Green,
            6 => Pen07Blue,
            7 => Pen08Yellow,
            8 => Pen09Orange,
            9 => Pen10Brown,
            10 => Pen11Pink,
            11 => Pen12DarkGrey,
            12 => Pen13Grey,
            13 => Pen14LightGreen,
            14 => Pen15LightBlue,
            15 => Pen16LightGrey,
            _ => throw new ArgumentOutOfRangeException("Index should be 0 to 15.")
        };

    public Brush GetBrush(int index) =>
        index switch
        {
            0 => Brush01Black,
            1 => Brush02White,
            2 => Brush03Red,
            3 => Brush04Cyan,
            4 => Brush05Purple,
            5 => Brush06Green,
            6 => Brush07Blue,
            7 => Brush08Yellow,
            8 => Brush09Orange,
            9 => Brush10Brown,
            10 => Brush11Pink,
            11 => Brush12DarkGrey,
            12 => Brush13Grey,
            13 => Brush14LightGreen,
            14 => Brush15LightBlue,
            15 => Brush16LightGrey,
            _ => throw new ArgumentOutOfRangeException("Index should be 0 to 15.")
        };

    public void Dispose()
    {
        Pen01Black.Dispose();
        Pen02White.Dispose();
        Pen03Red.Dispose();
        Pen04Cyan.Dispose();
        Pen05Purple.Dispose();
        Pen06Green.Dispose();
        Pen07Blue.Dispose();
        Pen08Yellow.Dispose();
        Pen09Orange.Dispose();
        Pen10Brown.Dispose();
        Pen11Pink.Dispose();
        Pen12DarkGrey.Dispose();
        Pen13Grey.Dispose();
        Pen14LightGreen.Dispose();
        Pen15LightBlue.Dispose();
        Pen16LightGrey.Dispose();
        Brush01Black.Dispose();
        Brush02White.Dispose();
        Brush03Red.Dispose();
        Brush04Cyan.Dispose();
        Brush05Purple.Dispose();
        Brush06Green.Dispose();
        Brush07Blue.Dispose();
        Brush08Yellow.Dispose();
        Brush09Orange.Dispose();
        Brush10Brown.Dispose();
        Brush11Pink.Dispose();
        Brush12DarkGrey.Dispose();
        Brush13Grey.Dispose();
        Brush14LightGreen.Dispose();
        Brush15LightBlue.Dispose();
        Brush16LightGrey.Dispose();
    }
}