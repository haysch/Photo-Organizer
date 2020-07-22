using System;
using System.IO;

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

        /// <summary>
        /// Ensures that the directory at given path exists.
        /// </summary>
        /// <param name="path">Path to directory.</param>
        /// <param name="message">Message to be thrown in exception.</param>
        /// <remarks>If message is not provided, throws a standard error message.</remarks>
        /// <exception cref="DirectoryNotFoundException">Thrown if the path is not an existing directory.</exception>
        public static void EnsureDirectoryExists(this string path, string? message = null)
        {
            if (!Directory.Exists(path))
            {
                if (message is null)
                {
                    message = $"Directory at { path } does not exist.";
                }

                throw new DirectoryNotFoundException(message);
            }
        }
    }
}