using System;
using System.IO;
using System.Collections.Generic;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            List<Image> imgList = new List<Image>();
            imgList.Add(new Image("fakeImg.jpg", "testdata"));
            imgList.Add(new Image("img1.jpg", "testdata"));
            imgList.Add(new Image("Food.jpg", "testdata"));
            imgList.Add(new Image("img2.jpg", "testdata"));

            foreach (Image img in imgList) {
                img.ExtractMetadata();
                i++;

                PrintMetadata(img, i, imgList.Count);
            }
        }

        private List<Image> LoadImages() {
            List<Image> imageList = new List<Image>();

            // TODO enumerate folders at i depth and 

            return imageList;
        }

        public static void PrintMetadata(Image image, int current, int max) {
            Console.WriteLine("================={0}/{1}=================", current, max);
            Console.WriteLine("DateTime Original: {0}", image.DateTimeOriginal);
            Console.WriteLine("ISO: {0}", image.ISO);
            Console.WriteLine("Aperture: {0}", image.Aperture);
            Console.WriteLine("F-Number: {0}", image.FNumber);
            Console.WriteLine("Shutter Speed: {0}", image.ShutterSpeed);
            Console.WriteLine("Focal Length: {0}", image.FocalLength);
            Console.WriteLine("Make: {0}\tModel: {1}", image.Make, image.Model);
            Console.WriteLine("Width: {0}\tHeight: {1}", image.Width, image.Height);
            Console.WriteLine("Lat/Long: {0:N6}, {1:N6}", image.Latitude, image.Longitude);
            Console.WriteLine("Altitude (meters): {0}", image.Altitude);
            Console.WriteLine("Hash value (MD5): {0}", image.HashValue);
            Console.WriteLine("=====================================\n");
        }
    }
}
