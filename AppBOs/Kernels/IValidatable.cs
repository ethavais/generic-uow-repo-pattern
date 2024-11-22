using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppBOs.Kernels
{
    public interface IValidatable
    {
        bool Validate(out string validationError);
    }

    public static class ValidationExtensions
    {
        public static bool IsValidFirstWord(this string? title)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length <= 10)
                return false;

            string[] words = title.Split(' ');
            var capitalizedWords = words.All(word =>
                char.IsUpper(word[0]) &&
                Regex.IsMatch(word, @"^[A-Za-z@$&()]*$"));

            return capitalizedWords;
        }

    }
}
