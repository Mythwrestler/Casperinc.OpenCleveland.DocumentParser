using System;
using System.Collections.Generic;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Models
{
    public class NeighborWords
    {
        public Dictionary<int, string> PreceedingWords { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> SucceedingWords { get;  set; } = new Dictionary<int, string>();
    }
}