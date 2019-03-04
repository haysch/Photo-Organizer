using PhotoOrganizer.DataModel;

namespace PhotoOrganizer.Util
{
    interface IRename
    {
        void RenameFile(string oldName, string newName, string folderPath);

        void RenameJpeg(ImageData image);
    }
}