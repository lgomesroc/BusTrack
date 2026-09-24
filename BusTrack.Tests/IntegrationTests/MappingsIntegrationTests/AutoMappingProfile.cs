using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.MappingsIntegrationTests
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusDB, BusDTOAPI>();
            CreateMap<BusDTOAPI, BusDB>();

            CreateMap<DriverDB, DriverDTOAPI>();
            CreateMap<DriverDTOAPI, DriverDB>();

            CreateMap<PassengerDB, PassengerDTOAPI>();
            CreateMap<PassengerDTOAPI, PassengerDB>();

            CreateMap<RouteDB, RouteDTOAPI>();
            CreateMap<RouteDTOAPI, RouteDB>();

            CreateMap<TripDB, TripDTOAPI>();
            CreateMap<TripDTOAPI, TripDB>();

            CreateMap<TripPassengerDB, TripPassengerDTOAPI>();
            CreateMap<TripPassengerDTOAPI, TripPassengerDB>();
        }
    }
}
