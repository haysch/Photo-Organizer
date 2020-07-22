using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PhotoOrganizerLib.Tests.Utils
{
    public static class PathHelper
    {
        public static string GetTemporaryDirectory()
        {
            // Generates temp directory path
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Creates temp directory
            Directory.CreateDirectory(tempDirectory);

            // Returns temp directory path
            return tempDirectory;
        }

        public static string CreateTmpFile(string path)
        {
            // Generates temp filename and path to file
            var tmpFileName = Guid.NewGuid().ToString();
            var tmpPath = Path.Combine(path, tmpFileName);

            // Creates the temp file
            File.WriteAllBytes(tmpPath, new byte[2] { 0xff, 0xff });

            // Returns temp filename
            return tmpPath;
        }

        public static string CreateImageFile(string path, ImageFormat imageFormat)
        {
            var tmpFileName = Guid.NewGuid().ToString();
            var tmpPath = Path.Combine(path, tmpFileName);

            using var bmp = new Bitmap(10, 10);
            bmp.Save(tmpPath, imageFormat);

            return tmpPath;
        }
    }
}
