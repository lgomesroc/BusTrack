using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.MappingsIntegrationTests
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusDB, BusDTOAPI>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.Plate));


       }
    }
}
