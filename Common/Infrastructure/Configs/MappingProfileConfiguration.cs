using AutoMapper;
using DapperApi.DTO;
using DapperApiDemo.Models;

namespace DapperApi.Common.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}