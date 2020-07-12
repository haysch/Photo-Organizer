using PhotoOrganizerLib.Interfaces;
using Microsoft.Extensions.Configuration;
using PhotoOrganizerLib.Utils;

namespace PhotoOrganizerLib.Services
{
    public class OrganizerService : IOrganizerService
    {
        ISortService _sortService;
        IConfiguration _configuration;

        public OrganizerService(IConfiguration configuration, ISortService sortService)
        {
            _configuration = configuration;
            _sortService = sortService;
        }

        public void RunOrganizer()
        {
            var startDirectory = _configuration["input"];
            var photoList = PhotoHandler.FindPhotos(startDirectory);
        }
    }
}