using System.IO;

namespace PhotoOrganizerLib.Interfaces
{
    public interface IChecksum
    {
        string? ComputeChecksum(Stream stream);
    }
}