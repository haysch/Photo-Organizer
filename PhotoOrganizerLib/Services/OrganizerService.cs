using MetadataExtractor;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Class for organizing media files.
    /// </summary>
    public class OrganizerService : IOrganizerService
    {
        private readonly ILogger<IOrganizerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly PhotoContext? _context;
        private readonly ISortService _sortService;

        /// <summary>
        /// Constructor for setting up organizer service.
        /// </summary>
        /// <param name="logger">Logger used for logging.</param>
        /// <param name="configuration">Configuration object containing information about settings.</param>
        /// <param name="context">Database context used for storing photo metadata.</param>
        /// <param name="sortService">Sort service used for sorting photos.</param>
        public OrganizerService(ILogger<IOrganizerService> logger, IConfiguration configuration, ISortService sortService, PhotoContext? context = null)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _sortService = sortService;
        }

        /// <summary>
        /// Runs the organizer by recursively searching every directory from the given <paramref name="inputDirectory" />.
        /// </summary>
        /// <param name="inputDirectory">Path to initial directory to search for media files.</param>
        /// <param name="database">Indicates whether a database is used.</param>
        public async Task RunOrganizerAsync(string inputDirectory)
        {
            // if input directory does not exist, throw exception and end run
            inputDirectory.EnsureDirectoryExists();

            // preliminary setup
            var hashAlgorithm = _configuration.GetValue<Algorithm>("hash-algorithm");
            var checksum = new Checksum(hashAlgorithm);

            var photoCounter = 0;
            if (!(_context is null))
            {
                await _context.Database.EnsureCreatedAsync();
            }

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

                // Add photo to database context if it does not exist already
                if (!(_context is null) && !await _context.Photos.AnyAsync(p => p.Name == photo.Name))
                {
                    await _context.Photos.AddAsync(photo);
                }

                photoCounter++;
            }

            if (!(_context is null))
            {
                // Save all additions to the database
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation($"End organizing. Organized { photoCounter } photos.");
        }
    }
}