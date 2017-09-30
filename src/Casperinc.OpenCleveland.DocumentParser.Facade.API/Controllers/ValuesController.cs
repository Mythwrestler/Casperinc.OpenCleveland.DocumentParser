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
            var paths = _documentRepo.GetObjectListForDirectory("/Users/Casper/Desktop/1996");
            
            //foreach (var path in paths)
            //{
            var document = await _documentRepo.GetDocumentFromPathAsync(paths.FirstOrDefault());
            //}

            return Ok("Ok");
        }

    }
}
