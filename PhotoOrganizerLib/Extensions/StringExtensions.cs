using System;

namespace PhotoOrganizerLib.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Checks whether string is a valid year.</summary>
        /// <param name="yearString">String of a potential year.</param>
        public static bool IsYear(this string yearString)
        {
            if (int.TryParse(yearString, out var year))
            {
                return DateTime.MinValue.Year <= year && year <= DateTime.MaxValue.Year;
            }

            return false;
        }

        /// <summary>Checks whether string is a valid month.</summary>
        /// <param name="monthString">String of a potential month.</param>
        public static bool IsMonth(this string monthString)
        {
            if (int.TryParse(monthString, out var month))
            {
                return DateTime.MinValue.Month <= month && month <= DateTime.MaxValue.Month;
            }

            return false;
        }
    }
}