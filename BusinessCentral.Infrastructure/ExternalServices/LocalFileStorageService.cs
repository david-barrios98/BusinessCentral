using BusinessCentral.Application.Ports.Outbound;
using Microsoft.Extensions.Configuration;

namespace BusinessCentral.Infrastructure.ExternalServices;
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    public LocalFileStorageService(IConfiguration config)
    {
        _basePath = config.GetValue<string>("FileStorage:BasePath") ?? "wwwroot/uploads";
        if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string container)
    {
        var dir = Path.Combine(_basePath, container);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var full = Path.Combine(dir, safeName);
        using var fs = File.Create(full);
        await fileStream.CopyToAsync(fs);
        // retorna ruta relativa para almacenar en BD
        return full;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath)) File.Delete(filePath);
        return Task.CompletedTask;
    }
}