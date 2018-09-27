using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Extensions
{
    public static class FileExtensions
    {
        public static List<string> FileTypes(this string path, string extension)
        {
            var dirInfo = new DirectoryInfo(path);
            var docFiles = dirInfo.EnumerateFiles().Where(fi => Path.GetExtension(fi.FullName) == extension);
            return docFiles.Select(fi => fi.FullName).ToList();
        }
    }
}
