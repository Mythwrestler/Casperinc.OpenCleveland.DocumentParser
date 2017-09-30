using AutoMapper;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Data.Models;
using Casperinc.OpenCleveland.DocumentParser.Core.Models;

namespace Casperinc.OpenCleveland.DocumentParser.Core.Helpers
{
    public class DataDTOMapperProfile : Profile
    {
        public DataDTOMapperProfile() : base("DataDTOMapperProfile")
        {
            CreateMap<Document, DocumentDataDTO>()
              .ForMember(dest => dest.GuidId, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash));
            CreateMap<DocumentDataDTO, Document>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.GuidId))
              .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash));
        } 
        
    }
}