using System;
using System.Collections.Generic;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Data
{
    public interface IDocumentSource
    {
        bool DocumentExists(string documentHash);
        bool DocumentExists(DocumentDataDTO document);
        DocumentDataDTO SaveDocument(DocumentDataDTO document);

    } 
}
