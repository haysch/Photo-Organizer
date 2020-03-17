using System;
using System.IO;

using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Models;
using PhotoOrganizerLib.Enums;

namespace PhotoOrganizerLib.Utils
{
    /// <summary>Renaming class for copying or moving of files.</summary>
    public class Rename : IRename
    {
        private RenameType _renameType;

        /// <summary>Constructor for renaming class. Sets up type used for renaming files.</summary>
        /// <param name="renameType">Type of move used for renaming. See <see cref="PhotoOrganizerLib.Enums.RenameType" /> for available types.</param>
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
        /// <remarks>Calls private renaming method for moving/copying of file. See <see cref="Rename.RenameFile" />.</remarks> 
        /// <param name="photo">The file as a <see cref="PhotoOrganizerLib.Models.Photo" /> object.</param>
        public void RenameImage(Photo photo)
        {
            var fileExt = Path.GetExtension(photo.PhotoName).ToLower();

            var folderPath = photo.AbsoluteFolderPath;
            var oldName = photo.PhotoName;
            var newName = string.Empty;

            // Only name according to DateTime Original
            if (photo.ImageMetadata.ContainsKey("ExifDTOrig"))
            {
                newName = ((DateTime) photo.ImageMetadata["ExifDTOrig"]).ToString("yyyyMMdd_HHmmss") + fileExt;
            }
            else
            {
                throw new ArgumentException($"Renaming not possible. No date/time data available for {photo.PhotoName}.");
            }

            try
            {
                RenameFile(oldName, newName, folderPath);
                photo.PhotoName = newName;
            }
            catch (Exception ex) //when (ex is FileNotFoundException || ex is ArgumentException)
            {
                throw ex;
            }
        }
    }
}