using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Helpers
{
    public static class StringExtensions
    {
        public static List<int> AllPositions(this string reviewText, string searchString)
        {
            //var regexPattern = $"@\b({searchString})\b";
            
            var positions = Regex.Matches(reviewText.Replace("\n", " ")
                            ,@"\b" + searchString + @"\b"
                            ,RegexOptions.IgnoreCase
                            )
                            .Cast<Match>()
                            .Select(m => m.Index)
                            .ToList();;

            return positions;
        }

    }
}