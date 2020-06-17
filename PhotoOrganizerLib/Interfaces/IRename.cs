using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IRename
    {
        string FindDateTimeName(Photo photo, string format);
        void RenameFile(string sourcePath, string targetPath);
    }
}