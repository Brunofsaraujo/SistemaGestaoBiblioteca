using System.Text.RegularExpressions;

namespace SistemaGestaoBiblioteca.Dominio.ValueObjects
{
    public class ValidadorISBN
    {
        public static bool EhRegistroValidoISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return false;

            isbn = Regex.Replace(isbn, @"[\s-]", "");

            return isbn.Length switch
            {
                10 => IsValidISBN10(isbn),
                13 => IsValidISBN13(isbn),
                _ => false
            };
        }

        private static bool IsValidISBN10(string isbn)
        {
            if (!isbn.All(char.IsDigit) && isbn[^1] != 'X') return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn[i])) return false;
                sum += (isbn[i] - '0') * (10 - i);
            }

            char lastChar = isbn[^1];
            sum += (lastChar == 'X') ? 10 : (lastChar - '0');

            return sum % 11 == 0;
        }

        private static bool IsValidISBN13(string isbn)
        {
            if (!isbn.All(char.IsDigit)) return false;

            int sum = 0;
            for (int i = 0; i < 13; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            return sum % 10 == 0;
        }
    }
}
