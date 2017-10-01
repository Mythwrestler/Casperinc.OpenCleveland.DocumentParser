using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Repositories
{
    public interface IDocumentRepository
    { 
        // Document BuildNewDocumentFromText(string fullText);
        Task<Document> GetDocumentFromPath(string path);
        List<string> GetObjectListForDirectory(string path);

    }


}
