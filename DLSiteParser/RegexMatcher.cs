using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DLSiteParser
{
    public static class RegexMatcher
    {
        public static string CleanUpText(string text)
        {
            return Regex.Replace(text, @"\s+", string.Empty);
        }

        public static string FindWorkNumber(string folderName)
        {
            var regexMatch = new Regex(@"RJ\d{5,6}");
            var matches = regexMatch.Matches(folderName);

            if (matches.Any())
                return matches.First().Value;

            Console.WriteLine($"Could not find RJ number. Please enter number for {folderName}. Leave empty to skip this folder.");
            var input = Console.ReadLine();
            if (input.Any())
                return FindWorkNumber(input);

            return string.Empty;
        }
    }
}
