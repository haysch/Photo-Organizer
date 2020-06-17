using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface ISort
    {
        void SortPhoto(Photo photo);
        void SortDateTimeFile(string sourcePath, string fileName);
        void SortUnknownFile(string sourcePath);
    }
}