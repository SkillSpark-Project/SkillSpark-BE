
using AutoMapper;
using Application.Commons;
using Domain.Entities;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.TagViewModels;

namespace Infrastructures.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<TagModel, Tag>().ReverseMap();
            /*             CreateMap<Chemical, ChemicalViewModel>()
                 .ForMember(dest => dest._Id, src => src.MapFrom(x => x.Id));*/
        }
    }
}
