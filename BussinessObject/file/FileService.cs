using Microsoft.AspNetCore.Http;

namespace BussinessObject.file
{
    public class FileService : IFileService
    {
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            // Tạo tên file ngẫu nhiên để tránh trùng lặp
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "Uploads", folderName, fileName);

            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn ảnh
            return $"/Content/Uploads/{folderName}/{fileName}";
        }
    }
}
