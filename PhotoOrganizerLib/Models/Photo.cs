using System;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizerLib.Models
{
    /// <summary>This class represents an image and contains functionality to extract metadata.</summary>
    public class Photo
    {
        /// <summary>
        /// Gets and sets the name of the image.
        /// </summary>
        [Key]
        public string Name { get; set; }
        /// <summary>
        /// Gets and sets the height of the image.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Gets and sets the width of the image.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets and sets the checksum of the image.
        /// </summary>
        public string? Checksum { get; set; }
        /// <summary>
        /// Gets and sets the latitude, where the image was taken.
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Gets and sets the longitude, where the image was taken.
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Gets and sets the altitude reference, where the image was taken.
        /// Sea level -or- Below sea level.
        /// </summary>
        public string? AltitudeReference { get; set; }
        /// <summary>
        /// Gets and sets the altitude, where the image was taken.
        /// </summary>
        public short? Altitude { get; set; }
        /// <summary>
        /// Gets and sets the make of the camera.
        /// </summary>
        public string? Make { get; set; }
        /// <summary>
        /// Gets and sets the model of the camera.
        /// </summary>
        public string? Model { get; set; }
        /// <summary>
        /// Gets and sets the date and time when the image was originally generated.
        /// </summary>
        public DateTime? DateTimeOriginal { get; set; }
        /// <summary>
        /// Gets and sets the date and time of the image was created.
        /// </summary>
        public DateTime? DateTime { get; set; }
        /// <summary>
        /// Gets and sets the F-number of the camera.
        /// </summary>
        public float? FNumber { get; set; }
        /// <summary>
        /// Gets and sets the ISO of the camera.
        /// </summary>
        public short? Iso { get; set; }
        /// <summary>
        /// Gets and sets the shutter speed of the camera.
        /// </summary>
        public string? ShutterSpeed { get; set; }
        /// <summary>
        /// Gets and sets the focal length of the camera.
        /// </summary>
        public float? FocalLength { get; set; }
        
        /// <summary>
        /// Gets and sets the directory path to the file excluding its name.
        /// </summary>
        [NotMapped]
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Gets and set the file path to the photo.
        /// </summary>
        [NotMapped]
        public string FilePath
        {
            get => Path.Combine(DirectoryPath, Name);

            set
            {
                Name = Path.GetFileName(value);
                DirectoryPath = Path.GetDirectoryName(value);
            }
        }

        /// <summary>
        /// Initializing a new instance of the <see cref="Photo" /> without a filepath.
        /// </summary>
        /// <remarks>Sets Name and DirectoryPath to <see langword="string.Empty" />.</remarks>
        public Photo()
        {
            Name = string.Empty;
            DirectoryPath = string.Empty;
        }

        /// <summary>
        /// Initializing a new instance of the <see cref="Photo" /> class using the <paramref name="filepath"/>.
        /// </summary>
        /// <param name="filepath">
        /// Absolute filepath to file.
        /// If <see langword="string.Empty" />, then Name is <see langword="string.Empty" /> and DirectoryPath is <see langword="null" />.
        /// If <see langword="null" />, then Name and DirectoryPath is <see langword="null" />.
        /// </param>
        public Photo(string filepath)
        {
            Name = Path.GetFileName(filepath);
            DirectoryPath = Path.GetDirectoryName(filepath);
        }
    }
}