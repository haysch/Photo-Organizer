using PhotoOrganizerLib.Interfaces;
using Microsoft.Extensions.Configuration;
using PhotoOrganizerLib.Utils;
using PhotoOrganizerLib.Extensions;
using System.IO;
using System.Collections.Generic;
using MetadataExtractor;
using PhotoOrganizerLib.Data;
using System.Threading.Tasks;

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

            await foreach (var photo in PhotoHandler.FindPhotosAsync(inputDirectory))
            {
                var metadataDirectories = FetchMetadataDirectories(photo.FilePath);
                ParseMetadata.Parse(photo, metadataDirectories);
                _sortService.SortPhoto(photo);

                await _context.AddAsync(photo);
            }

            await _context.SaveChangesAsync();
        }

        private static IEnumerable<MetadataExtractor.Directory> FetchMetadataDirectories(string path)
        {
            using var fs = File.OpenRead(path);
            return ImageMetadataReader.ReadMetadata(fs);
        }
    }
}