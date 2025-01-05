using System.Globalization;
using System.Text;

namespace Eleven.BookManager.Business.Extensions
{
    public static partial class StringExtensions
    {
        public static string Clear(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : new string(
                value.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray()
            ).Normalize(NormalizationForm.FormC);
        }

        public static string GetValidFileName(this string fileName)
        {
            var invalidCharacters = new List<char> { '¿' };
            var extension = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);

            invalidCharacters.AddRange(Path.GetInvalidFileNameChars());

            name = string.Concat(name.Split([.. invalidCharacters]));
            return string.Concat(name, extension);
        }
    }
}
