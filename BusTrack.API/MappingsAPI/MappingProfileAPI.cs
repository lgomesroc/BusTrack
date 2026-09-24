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

            CreateMap<BusDB, BusDTOAPI>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Number,
                    options => options.MapFrom(
                        source => source.Number))
                .ForMember(
                    destination => destination.LicensePlate,
                    options => options.MapFrom(
                        source => source.Plate))
                .ForMember(
                    destination => destination.Model,
                    options => options.MapFrom(
                        source => source.Line))
                .ForMember(
                    destination => destination.Capacity,
                    options => options.MapFrom(
                        source => source.Capacity));

            CreateMap<BusDTOAPI, BusDB>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Number,
                    options => options.MapFrom(
                        source => source.Number))
                .ForMember(
                    destination => destination.Plate,
                    options => options.MapFrom(
                        source => source.LicensePlate))
                .ForMember(
                    destination => destination.Line,
                    options => options.MapFrom(
                        source => source.Model))
                .ForMember(
                    destination => destination.Capacity,
                    options => options.MapFrom(
                        source => source.Capacity))
                .ForMember(
                    destination => destination.Routes,
                    options => options.Ignore());

            CreateMap<DriverDB, DriverDTOAPI>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.LicenseNumber,
                    options => options.MapFrom(
                        source => source.Cpf));

            CreateMap<DriverDTOAPI, DriverModelAPI>()
                .ForMember(
                    destination => destination.Id,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.LicenseNumber,
                    options => options.MapFrom(
                        source => source.LicenseNumber));

            CreateMap<DriverModelAPI, DriverDB>()
                .ForMember(
                    destination => destination.Id,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Cpf,
                    options => options.MapFrom(
                        source => source.LicenseNumber))
                .ForMember(
                    destination => destination.Login,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Email,
                    options => options.Ignore());

            CreateMap<DriverDTOAPI, DriverDB>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Cpf,
                    options => options.MapFrom(
                        source => source.LicenseNumber))
                .ForMember(
                    destination => destination.Login,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Email,
                    options => options.Ignore());

            CreateMap<RouteDB, RouteDTOAPI>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Origin,
                    options => options.MapFrom(
                        source => source.Origin))
                .ForMember(
                    destination => destination.Destination,
                    options => options.MapFrom(
                        source => source.Destination))
                .ForMember(
                    destination => destination.Description,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Distance,
                    options => options.Ignore());

            CreateMap<RouteDTOAPI, RouteDB>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(
                        source => source.Id))
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Origin,
                    options => options.MapFrom(
                        source => source.Origin))
                .ForMember(
                    destination => destination.Destination,
                    options => options.MapFrom(
                        source => source.Destination));
        }
    }
}
