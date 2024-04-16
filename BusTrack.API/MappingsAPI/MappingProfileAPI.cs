using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ModelsAPI;

namespace BusTrack.BusTrack.API.MappingsAPI
{
    public class MappingProfileAPI : Profile
    {
        public void MappingProfile()
        {
            CreateMap<BusModelAPI, BusDTOAPI>();
            CreateMap<RouteModelAPI, RouteDTOAPI>();
            CreateMap<TripModelAPI, TripDTOAPI>();
            CreateMap<PassengerModelAPI, PassengerDTOAPI>();
            CreateMap<DriverModelAPI, DriverDTOAPI>();
            CreateMap<TripsPassengerModelAPI, TripPassengerDTOAPI>();
        }
    }
}
