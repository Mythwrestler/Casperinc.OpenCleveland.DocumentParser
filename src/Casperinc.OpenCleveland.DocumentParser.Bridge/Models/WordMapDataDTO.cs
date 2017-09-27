using System;
using System.Collections.Generic;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Models
{
    public class WordMapDataDTO
    {
        public Guid GuidId {get; set;}
        public WordDataDTO Word {get; set;}
        public List<int> Positions {get; set;} = new List<int>();
    }
}