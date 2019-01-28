using System;
using System.IO;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Image test = new Image("fakeImg.jpg", "testdata");
            Image test2 = new Image("img1.jpg", "testdata");
            Image test3 = new Image("Food.jpg", "testdata");
            Image test4 = new Image("img2.jpg", "testdata");

            // test.extractDateTime();
            // test2.extractDateTime();
            // test4.extractDateTime();
            // test3.extractDateTime();

            test2.extractLatitude();
            test2.extractLongitude();

            Console.WriteLine("Lat: {0}\tLong: {1}", test2.Latitude, test2.Longitude);

            test3.extractLatitude();
            test3.extractLongitude();

            Console.WriteLine("Lat: {0}\tLong: {1}", test3.Latitude, test3.Longitude);

            test4.extractLatitude();
            test4.extractLongitude();

            Console.WriteLine("Lat: {0}\tLong: {1}", test4.Latitude, test4.Longitude);
            
            // test4.extractModel();
            
            // test4.GetResolution();

            // Console.WriteLine("Make: {0}\nModel: {1}\nWidth: {2}\nHeight: {3}", test4.Make, test4.Model, test4.Width, test4.Height);

            //Console.WriteLine(test4.Longitude);
            // Console.WriteLine(test3.DateTimeOriginal);
            // Console.WriteLine(test4.Aperture);
            // Console.WriteLine(test2.DateTimeOriginal);
        }
    }
}
