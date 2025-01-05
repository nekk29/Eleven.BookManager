using Eleven.BookManager.Business.Models.Configuration;

namespace Eleven.BookManager.Business.Extensions
{
    public static class CalibreSettingsExtensions
    {
        public static string GetRelativePath(this CalibreOptions calibreOptions, string filePath)
        {
            filePath = filePath.Replace(calibreOptions.LibraryDirectory, string.Empty);
            filePath = filePath.StartsWith('/') ? filePath[1..] : filePath;
            filePath = filePath.StartsWith('\\') ? filePath[1..] : filePath;

            return filePath;
        }
    }
}
