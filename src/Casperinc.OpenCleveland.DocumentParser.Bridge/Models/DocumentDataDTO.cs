using System;
using System.Collections.Generic;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Models
{
    public class DocumentDataDTO
    {
        public Guid? GuidId { get; set; }
        public string Hash { get; set; }
        public string FullText { get; set;}
        public List<WordMapDataDTO> WordMaps {get; set;}
    }
}