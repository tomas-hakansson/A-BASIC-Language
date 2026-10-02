#nullable enable
using System;

namespace A_BASIC_Language.StringManipulation;

public static class StringExtensions
{
    extension(string? me)
    {
        public bool IsEmpty() =>
            string.IsNullOrWhiteSpace(me);

        public string MaxLength(int min, int max)
        {
            if (me == null)
                return new string(' ', min);

            if (me.Length == min)
                return me;

            if (me.Length < min)
                return me + new string(' ', min - me.Length);

            if (me.Length > max)
                return me.Substring(0, max);

            return me;
        }

        public bool IsTrue()
        {
            var v = (me ?? "").Trim().ToLower();
            return v == "yes" || v == "true" || v == "1" || v == "on";
        }

        public bool IsFalse()
        {
            var v = (me ?? "").Trim().ToLower();
            return v is "no" or "false" or "0" or "off";
        }

        public bool Is(string? other)
        {
            if (IsEmpty(me) && IsEmpty(other))
                return true;

            if (IsEmpty(me) || IsEmpty(other))
                return false;

            return string.Compare(me!.Trim(), other!.Trim(), StringComparison.CurrentCultureIgnoreCase) == 0;
        }
    }
}