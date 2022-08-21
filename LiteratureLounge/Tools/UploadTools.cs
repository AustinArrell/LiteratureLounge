using Microsoft.AspNetCore.Mvc;

namespace LiteratureLounge.Tools
{
    public class UploadTools
    {
        public static bool IsFileTypeValid(string filename, string[] permittedExtensions)
        {
            var ext = Path.GetExtension(filename).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }
            return true;
        }

        public static void DeleteIfExists(string filePath) 
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
