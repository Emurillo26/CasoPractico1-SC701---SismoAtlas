using System.Text.RegularExpressions;

namespace SismoAtlas.API.Validation
{
    
    // solo letras (con tildes), espacios, punto, apóstrofe y guion; máximo 100 caracteres.
    public static class CountryNameValidator
    {
        public const int MaxLength = 100;

        private static readonly Regex NameRegex = new(@"^[\p{L}\s.'\-]+$", RegexOptions.Compiled);

        public static bool IsValid(string? name)
        {
            return !string.IsNullOrWhiteSpace(name)
                   && name.Length <= MaxLength
                   && NameRegex.IsMatch(name);
        }
    }
}