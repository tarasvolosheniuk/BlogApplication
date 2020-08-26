using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSauce.MagicScaler;
namespace BlogApplication.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private string _imagePath;

        public FileManager(IConfiguration configuration)
        {
            _imagePath = configuration["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                {
                        File.Delete(file);
                        return true;
                }
            }
            catch (Exception e )
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return false;
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {

           
            var savePath = Path.Combine(_imagePath);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //IE Error
            //var fileName = image.FileName
            var mime = image.FileName.Substring(image.FileName.IndexOf('.'));
            var filename = $"img_{DateTime.Now.ToString("dd-MM-yyyy-ss-mm")}{mime}";
            using (var fileStream = new FileStream(Path.Combine(savePath,filename),FileMode.Create))
            {
               //await image.CopyToAsync(fileStream);
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageOptions());
            }
            return filename;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return "Error";
            }
        }
        private ProcessImageSettings ImageOptions()=>new ProcessImageSettings { 
        Width = 800,
        Height = 500,
        ResizeMode= CropScaleMode.Crop,
        SaveFormat=FileFormat.Jpeg,
        JpegQuality = 100,
        JpegSubsampleMode = ChromaSubsampleMode.Subsample420,
        };
    }
}
