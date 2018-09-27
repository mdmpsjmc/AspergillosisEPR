using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class FileImporter
    {
        public static void Import(IFormFile file, string webRootPath, Action<FileStream, IFormFile, string> action)
        {
            string folderName = "Upload";
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    action(stream, file, fileExtension);
                }
            }
        }

        public static void Import(string fullPath, Action<FileStream, string> action)
        {
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                action(stream, fullPath);
            }

        }
    }
}
