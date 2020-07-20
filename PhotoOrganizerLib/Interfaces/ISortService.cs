using System;
using System.Globalization;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface ISortService
    {
        void SortPhoto(Photo photo, string dateTimeFormat = "yyyyMMdd_HHmmss");
        void SortDateTime(string sourcePath,
            string dateTimeFileName,
            string dateTimeFormat = "yyyyMMdd_HHmmss",
            CultureInfo? provider = null,
            DateTimeStyles dateTimeStyles = DateTimeStyles.None);
    }
}