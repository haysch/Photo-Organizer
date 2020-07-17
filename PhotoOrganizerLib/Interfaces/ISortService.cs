using System;
using System.Globalization;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface ISortService
    {
        void SortPhoto(Photo photo);
        void SortDateTime(string sourcePath,
            string dateTimeFileName,
            string format,
            CultureInfo provider,
            DateTimeStyles dateTimeStyles);
    }
}