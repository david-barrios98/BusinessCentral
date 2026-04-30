namespace BusinessCentral.Application.Ports.Outbound;
public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string container);
    Task DeleteFileAsync(string filePath);
}