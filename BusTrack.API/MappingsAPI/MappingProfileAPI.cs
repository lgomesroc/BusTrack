using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.MappingsAPI
{
    public class MappingProfileAPI : Profile
    {
        public MappingProfileAPI()
        {
            CreateMap<TripDB, TripDTOAPI>();
            CreateMap<TripDTOAPI, TripDB>();

            CreateMap<TripDTOAPI, TripModelAPI>();
            CreateMap<TripModelAPI, TripDTOAPI>();

            CreateMap<TripPassengerDB, TripPassengerDTOAPI>();
            CreateMap<TripPassengerDTOAPI, TripPassengerDB>();
        }
    }
}