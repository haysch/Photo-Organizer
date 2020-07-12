using Microsoft.Extensions.Configuration;

namespace PhotoOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"{System.IO.Directory.GetCurrentDirectory()}/appsettings.json", false) // TODO is there any point in using a settings file?
                .AddEnvironmentVariables("PHORG_")
                .AddCommandLine(args);
        }
    }
}
