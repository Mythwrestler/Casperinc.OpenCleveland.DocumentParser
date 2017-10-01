using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casperinc.OpenCleveland.DocumentParser.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Casperinc.OpenCleveland.DocumentParser.Facade.API.Controllers
{
    [Route("api/documents")]
    public class ValuesController : Controller


    {
        private IDocumentRepository _documentRepo;

        public ValuesController(IDocumentRepository documentRepo)
        {
            _documentRepo = documentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> DoStuffAsync(){
            var paths = _documentRepo.GetObjectListForDirectory("/Users/Mat/Desktop/1996");
            
            foreach (var path in paths)
            {
                if(path.ToLowerInvariant().EndsWith(".txt"))
                {
                    var document = await _documentRepo.GetDocumentFromPath(path);
                }
            }

            return Ok("Ok");
        }

    }
}
