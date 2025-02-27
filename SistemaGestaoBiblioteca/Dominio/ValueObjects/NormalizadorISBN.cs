using System.Text.RegularExpressions;

namespace SistemaGestaoBiblioteca.Dominio.ValueObjects
{
    public class NormalizadorISBN
    {
        public static string NormalizarISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return string.Empty;

            return Regex.Replace(isbn, @"[^0-9X]", "", RegexOptions.IgnoreCase);
        }
    }
}
