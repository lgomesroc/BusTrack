using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.Tests.MappingsIntegrationTests
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusDB, BusDTOAPI>()
                .ForMember(
                    destination => destination.LicensePlate,
                    options => options.MapFrom(
                        source => source.Plate))
                .ForMember(
                    destination => destination.Model,
                    options => options.MapFrom(
                        source => source.Line));

            CreateMap<BusDTOAPI, BusDB>()
                .ForMember(
                    destination => destination.Plate,
                    options => options.MapFrom(
                        source => source.LicensePlate))
                .ForMember(
                    destination => destination.Line,
                    options => options.MapFrom(
                        source => source.Model))
                .ForMember(
                    destination => destination.Routes,
                    options => options.Ignore());

            CreateMap<DriverDB, DriverDTOAPI>()
                .ForMember(
                    destination => destination.LicenseNumber,
                    options => options.MapFrom(
                        source => source.Cpf));

            CreateMap<DriverDTOAPI, DriverDB>()
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
                    destination => destination.Login,
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
                    destination => destination.Email,
                    options => options.Ignore());

            CreateMap<PassengerDB, PassengerDTOAPI>();

            CreateMap<PassengerDTOAPI, PassengerDB>();

            CreateMap<PassengerDTOAPI, PassengerModelAPI>()
                .ForMember(
                    destination => destination.Id,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Age,
                    options => options.MapFrom(
                        source => source.Age));

            CreateMap<PassengerModelAPI, PassengerDB>()
                .ForMember(
                    destination => destination.Id,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(
                        source => source.Name))
                .ForMember(
                    destination => destination.Cpf,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Email,
                    options => options.Ignore())
                .ForMember(
                    destination => destination.Phone,
                    options => options.Ignore());

            CreateMap<RouteDB, RouteDTOAPI>();

            CreateMap<RouteDTOAPI, RouteDB>();

            CreateMap<TripDB, TripDTOAPI>();

            CreateMap<TripDTOAPI, TripDB>();

            CreateMap<TripPassengerDB, TripPassengerDTOAPI>();

            CreateMap<TripPassengerDTOAPI, TripPassengerDB>();
        }
    }
}
