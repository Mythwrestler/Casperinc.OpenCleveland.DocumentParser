using System;
using System.Collections.Generic;
using System.Linq;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Helpers
{
    public static class StringExtensions
    {
        public static List<int> AllPositions(this string reviewText, string searchString)
        {
            var positions = new List<int>();
            int pos = 0;
            while (reviewText.IndexOf(searchString, pos) != -1)
            {
                positions.Add(reviewText.IndexOf(searchString, pos));
                pos = positions.Last() + 1;
            }

            return positions;
        }

    }
}