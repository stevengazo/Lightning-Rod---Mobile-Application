using Manchito.DataBaseContext;
using Manchito.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Services
{
    public class PhotoService
    {
        private readonly string _basePath;

        public PhotoService(string basePath)
        {
            _basePath = basePath;
        }

        public async Task<string> SavePhotoAsync(FileResult photo, string folderPath)
        {
            if (photo == null) return null;

            string fileName = $"IMG_{DateTime.Now:yyyyMMdd_HHmmssfff}.jpg";
            string localFilePath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);
            File.Copy(photo.FullPath, localFilePath, true);

            return localFilePath;
        }

        public async Task RegisterPhotoAsync(string filePath, int categoryId, GPSLocation gps)
        {
            await using var db = new DBLocalContext();
            int lastId = db.Photography.OrderByDescending(p => p.PhotographyId)
                                       .Select(p => p.PhotographyId)
                                       .FirstOrDefault() + 1;

            var photo = new Photography
            {
                PhotographyId = lastId,
                FilePath = filePath,
                Name = Path.GetFileName(filePath),
                CategoryId = categoryId,
                DateTaked = DateTime.Now,
                Latitude = gps?.Latitude ?? 0,
                Longitude = gps?.Longitude ?? 0,
                Altitude = gps?.Altitude ?? 0
            };

            db.Photography.Add(photo);
            await db.SaveChangesAsync();
        }

        public async Task DeletePhotoAsync(int id)
        {
            await using var db = new DBLocalContext();
            var photo = db.Photography.FirstOrDefault(p => p.PhotographyId == id);
            if (photo != null)
            {
                db.Photography.Remove(photo);
                await db.SaveChangesAsync();
                if (File.Exists(photo.FilePath))
                    File.Delete(photo.FilePath);
            }
        }

        public async Task<List<Photography>> GetPhotosAsync(int categoryId)
        {
            await using var db = new DBLocalContext();
            return db.Photography.Where(p => p.CategoryId == categoryId)
                                 .OrderByDescending(p => p.PhotographyId)
                                 .ToList();
        }
    }
}
