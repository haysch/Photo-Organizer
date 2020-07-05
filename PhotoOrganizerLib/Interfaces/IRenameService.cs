using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IRenameService
    {
        string FindDateTimeName(Photo photo, string format);
        void RenameFile(string sourcePath, string targetPath);
    }
}