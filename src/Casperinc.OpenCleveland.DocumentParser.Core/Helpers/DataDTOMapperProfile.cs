using AutoMapper;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Helpers
{
    public class DataDTOMapperProfile : Profile
    {
        public DataDTOMapperProfile() : base("DataDTOMapperProfile")
        {
            CreateMap<Document, DocumentDataDTO>();
            CreateMap<DocumentDataDTO, Document>();
        } 
        
    }
}