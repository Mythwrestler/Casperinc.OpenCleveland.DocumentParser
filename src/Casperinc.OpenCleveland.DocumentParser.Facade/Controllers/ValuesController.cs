using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casperinc.OpenCleveland.DocumentParser.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Casperinc.OpenCleveland.DocumentParser.Facade.Controllers
{
    [Route("api/documents")]
    public class ValuesController : Controller


    {
        private IDocumentRepository _documentRepo;

        public ValuesController(IDocumentRepository documentRepo)
        {
            _documentRepo = documentRepo;
        }

    }
}
