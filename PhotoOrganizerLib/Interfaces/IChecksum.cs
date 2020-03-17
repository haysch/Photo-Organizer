namespace PhotoOrganizerLib.Interfaces
{
    public interface IChecksum
    {
        string ComputeChecksum(string filePath);
    }
}