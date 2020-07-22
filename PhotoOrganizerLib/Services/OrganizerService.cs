using MetadataExtractor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoOrganizerLib.Data;
using PhotoOrganizerLib.Enums;
using PhotoOrganizerLib.Extensions;
using PhotoOrganizerLib.Interfaces;
using PhotoOrganizerLib.Utils;
using System.IO;
using System.Threading.Tasks;

namespace PhotoOrganizerLib.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly ILogger<IOrganizerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly PhotoContext _context;
        private readonly ISortService _sortService;

        public OrganizerService(ILogger<IOrganizerService> logger, IConfiguration configuration, PhotoContext context, ISortService sortService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _sortService = sortService;
        }

        public async Task RunOrganizerAsync(string inputDirectory)
        {
            // if input directory does not exist, throw exception and end run
            inputDirectory.EnsureDirectoryExists();

            var hashAlgorithm = _configuration.GetValue<Algorithm>("hash-algorithm");
            var checksum = new Checksum(hashAlgorithm);

            var photoCounter = 0;

            _logger.LogInformation($"Begin organizing in { inputDirectory }");

            await foreach (var photo in PhotoHandler.FindPhotosAsync(inputDirectory))
            {
                using var fs = File.OpenRead(photo.FilePath);

                // Compute checksum
                photo.Checksum = checksum.ComputeChecksum(fs);

                // Reset filestream position
                fs.Position = 0;

                // Fetch metadata directories using MetadataExctractor and parse metadata to the Photo object
                var metadataDirectories = ImageMetadataReader.ReadMetadata(fs);
                ParseMetadata.Parse(photo, metadataDirectories);

                // Rename and sort photos
                _sortService.SortPhoto(photo);

                // Add photo to database context
                await _context.Photos.AddAsync(photo);

                photoCounter++;
            }

            // Save all additions to the database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"End organizing. Organized { photoCounter } photos.");
        }
    }
}