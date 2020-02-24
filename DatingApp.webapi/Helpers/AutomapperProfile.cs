using System.Linq;
using AutoMapper;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Model;

namespace DatingApp.webapi.Helpers
{
    public class AutomapperProfile: Profile
    {
        public AutomapperProfile()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest=> dest.PhotoUrl, 
                    opt=> opt.MapFrom(src=> src.Photos.FirstOrDefault(p=> p.IsMain).Url)
                )
                .ForMember(dest=> dest.Age,
                    opt=> opt.MapFrom(src=> src.DateOfBirth.CalculateAge())
                );

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest=> dest.PhotoUrl, 
                    opt=> opt.MapFrom(src=> src.Photos.FirstOrDefault(p=> p.IsMain).Url)
                )
                .ForMember(dest=> dest.Age,
                    opt=> opt.MapFrom(src=> src.DateOfBirth.CalculateAge())
                );

            CreateMap<Photo, PhotoForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotForCreateDto, Photo>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<UserForRegistrationDto, User>();
        }        
    }
}