using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public interface IImageService
{

    Task<string> SaveImageAsync(IFormFile imageFile, string folderName);

}
//with help of chat gpt to avoid repeating functionality 
public class ImageService(IWebHostEnvironment env) : IImageService
{
    private readonly IWebHostEnvironment _env = env;

    public async Task<string> SaveImageAsync(IFormFile imageFile, string folderName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }
        
            // Define the path where the images will be stored

            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);
        

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile!.CopyToAsync(stream);
        }

        // Return the relative path of the image
        return Path.Combine("uploads", folderName, fileName);
    }
}

