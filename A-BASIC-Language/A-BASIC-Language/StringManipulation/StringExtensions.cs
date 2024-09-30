namespace A_BASIC_Language.StringManipulation;

public static class StringExtensions
{
    public static bool IsEmpty(this string? me) =>
        string.IsNullOrWhiteSpace(me);

    public static string MaxLength(this string? me, int min, int max)
    {
        if (me == null)
            return new string(' ', min);

        if (me.Length == min)
            return me;

        if (me.Length < min)
            return me + new string(' ', min - me.Length);

        if (me.Length > max)
            return me[..max];

        return me;
    }

    public static bool IsTrue(this string? me)
    {
        var v = (me ?? "").Trim().ToLower();
        return v == "yes" || v == "true" || v == "1" || v == "on";
    }

    public static bool IsFalse(this string? me)
    {
        var v = (me ?? "").Trim().ToLower();
        return v == "no" || v == "false" || v == "0" || v == "off";
    }

    public static bool Is(this string? me, string? other)
    {
        if (IsEmpty(me) && IsEmpty(other))
            return true;

        if (IsEmpty(me) || IsEmpty(other))
            return false;

        return string.Compare(me!.Trim(), other!.Trim(), StringComparison.CurrentCultureIgnoreCase) == 0;
    }
}