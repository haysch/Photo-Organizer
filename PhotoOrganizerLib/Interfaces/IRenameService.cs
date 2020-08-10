using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Models;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IRenameService
    {
        RenameType RenameType { get; }
        string FindPhotoDateTime(Photo photo, string format);
        void RenameFile(string sourcePath, string destPath);
    }
}