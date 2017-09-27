using System;
using System.Collections.Generic;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Models
{
    public class WordMap
    {
        public Guid Id {get; set;}
        public Word Word { get; set; }
        public List<int> Positions { get; set; } = new List<int>();
        public NeighborWords NeighborWords { get; set;}


    }
}