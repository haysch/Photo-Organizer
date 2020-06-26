using System;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface ISort
    {
        void SortPhoto(Photo photo);
        void SortDateTime(string sourcePath, string dateTimeFileName);
        void SortDateTime(string sourcePath, DateTime dateTimeFileName);
    }
}