namespace PhotoOrganizer.Util
{
    interface IRename
    {
        void RenameFile(string oldName, string newName);

        void SetInternalImageName(ImageData image);
    }
}