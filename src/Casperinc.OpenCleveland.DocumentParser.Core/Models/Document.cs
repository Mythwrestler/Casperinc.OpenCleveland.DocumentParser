using System;
using System.Collections.Generic;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Models
{
    public class Document
    {
        public Guid Id {get; set;}
        public string Hash {get; set;}
        public string FullText { get; set;}
        public List<WordMap> WordMaps { get; set;}


    }
}