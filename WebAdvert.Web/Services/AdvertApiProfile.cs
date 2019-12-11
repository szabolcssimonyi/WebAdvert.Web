using AutoMapper;

namespace WebAdvert.Web.Services
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<AdvertApi.Models.AdvertModel, Models.AdvertModel>().ReverseMap();
            CreateMap<AdvertApi.Models.CreateAdvertResponse, Models.CreateAdvertResponse>().ReverseMap();
            CreateMap<AdvertApi.Models.ConfirmAdvertModel, Models.ConfirmAdvertModel>().ReverseMap();
        }
    }
}
