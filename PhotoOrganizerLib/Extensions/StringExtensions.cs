using System;

namespace PhotoOrganizerLib.Extensions
{
    public static class StringExtensions
    {
        public static bool IsYear(this string name)
        {
            if (int.TryParse(name, out var year))
            {
                return DateTime.MinValue.Year <= year && year <= DateTime.MaxValue.Year;
            }

            return false;
        }

        public static bool IsMonth(this string name)
        {
            if (int.TryParse(name, out var month))
            {
                return DateTime.MinValue.Month <= month && month <= DateTime.MaxValue.Month;
            }

            return false;
        }
    }
}