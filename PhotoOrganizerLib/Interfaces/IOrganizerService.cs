using System.Threading.Tasks;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IOrganizerService
    {
        Task RunOrganizerAsync(string inputDirectory);
    }
}