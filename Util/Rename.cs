using System;
using System.IO;

namespace PhotoOrganizer.Util
{
    /// <summary>TODO</summary>
    public enum RenameType
    {
        /// <summary>Copy file.</summary>
        Copy,
        /// <summary>Move file.</summary>
        Move
    }
    
    /// <summary>TODO</summary>
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
        /// <param name="oldName">The current filename.</param>
        /// <param name="newName">The new filename.</param>
        /// <exception cref="System.InvalidCastException">Thrown when the rename type is not valid.</exception>
        public void RenameFile(string oldName, string newName)
        {
            switch (_renameType)
            {
                case RenameType.Copy:
                    File.Copy(oldName, newName);
                    return;
                case RenameType.Move:
                    File.Move(oldName, newName);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetInternalImageName(ImageData image) {
            
        }
    }
}