using PhotoOrganizerLib.Interfaces;
using Microsoft.Extensions.Configuration;
using PhotoOrganizerLib.Utils;
using PhotoOrganizerLib.Extensions;
using System.IO;
using MetadataExtractor;
using PhotoOrganizerLib.Data;
using System.Threading.Tasks;
using PhotoOrganizerLib.Enums;
using System;

namespace PhotoOrganizerLib.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoContext _context;
        private readonly ISortService _sortService;

        public OrganizerService(IConfiguration configuration, PhotoContext context, ISortService sortService)
        {
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
            }

            // Save all additions to the database
            await _context.SaveChangesAsync();
        }
    }
}