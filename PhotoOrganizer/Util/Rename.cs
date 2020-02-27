using System;
using System.IO;

using PhotoOrganizer.Models;

namespace PhotoOrganizer.Util
{
    /// <summary>Specifies the available renaming types.</summary>
    public enum RenameType
    {
        /// <summary>Copy file.</summary>
        Copy,
        /// <summary>Move file.</summary>
        Move,
        /// <summary>Replace file.</summary>
        // TODO implement
        Replace,
        /// <summary>Renaming not wanted.</summary>
        None
    }

    /// <summary>Renaming class for copying or moving of files.</summary>
    public class Rename : IRename
    {
        private RenameType _renameType;

        /// <summary>Constructor for renaming class. Sets up type used for renaming files.</summary>
        /// <param name="renameType">Type of move used for renaming. See <see cref="RenameType" /> for available types.</param>
        public Rename(RenameType renameType)
        {
            _renameType = renameType;
        }

        /// <summary>Renames file by copying or moving it.</summary>
        /// <remarks>Renames files relative to the working directory, by default, if folderPath is not set.</remarks>
        /// <param name="oldName">The current filename.</param>
        /// <param name="newName">The new filename.</param>
        /// <param name="folderPath">Working directory, relative or absolute.</param>
        /// <exception cref="System.ArgumentException">Thrown when the rename type is not valid.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when the file to be renamed does not exist.</exception>
        private void RenameFile(string oldName, string newName, string folderPath = ".")
        {
            oldName = folderPath + Path.DirectorySeparatorChar + oldName;
            newName = folderPath + Path.DirectorySeparatorChar + newName;

            if (!File.Exists(oldName))
                throw new FileNotFoundException($"File with path {oldName} not found.");

            switch (_renameType)
            {
                case RenameType.Copy:
                    File.Copy(oldName, newName);
                    return;
                case RenameType.Move:
                    File.Move(oldName, newName);
                    return;
                case RenameType.Replace:
                    File.Replace(oldName, newName, oldName + ".backup");
                    return;
                case RenameType.None:
                    return;
                default:
                    throw new ArgumentException($"Renaming type {_renameType} is invalid");
            }
        }

        /// <summary>Renames image files by extracting the necessary information from the ImageData object.</summary>
        /// <remarks>Calls private renaming method for moving/copying of file. See <see cref="RenameFile" />.</remarks> 
        /// <param name="image">The JPEG file as an ImageData object. See <see cref="Picture" />.</param>
        public void RenameImage(Picture image)
        {
            string fileExt = Path.GetExtension(image.ImageName).ToLower();

            if (fileExt != ".jpg" && fileExt != ".jpeg")
                throw new ArgumentException($"Wrong file type: {fileExt} is not JPEG");

            string folderPath = image.AbsoluteFolderPath;
            string oldName = image.ImageName;
            string newName;

            if (image.ImageMetadata.ContainsKey("ExifDTOrig"))
                newName = ((DateTime)image.ImageMetadata["ExifDTOrig"]).ToString("yyyyMMdd_HHmmss");
            else if (image.ImageMetadata.ContainsKey("DateTime")) // TODO consider not copying since we cannot ensure correct DateTime for image naming
                newName = ((DateTime)image.ImageMetadata["DateTime"]).ToString("yyyyMMdd_HHmmss");
            else
                throw new ArgumentException($"Renaming not possible. No date/time data available for {image.ImageName}.");

            newName += fileExt;

            try
            {
                RenameFile(oldName, newName, folderPath);
                image.ImageName = newName;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is ArgumentException)
            {
                throw ex;
            }
        }
    }
}