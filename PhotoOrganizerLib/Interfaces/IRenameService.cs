using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IRenameService
    {
        string FindPhotoDateTime(Photo photo, string format);
        void RenameFile(string sourcePath, string targetPath);
    }
}