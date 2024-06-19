
using AutoMapper;
using Application.Commons;
using Domain.Entities;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.TagViewModels;
using Application.ViewModels.MentorViewModels.Requests;
using Application.ViewModels.CourseViewModels.Requests;
using Application.ViewModels.UserViewModels.Responses;
using Application.ViewModels.ChapterViewModels;
using Application.ViewModels.LessonViewModels.Requests;

namespace Infrastructures.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<TagModel, Tag>().ReverseMap();
            CreateMap<MentorModel, Mentor>().ReverseMap();
            CreateMap<CourseModel, Course>().ReverseMap();
            CreateMap<UserViewModel, ApplicationUser>().ReverseMap();
            CreateMap<ChapterModel, Chapter>().ReverseMap();
            CreateMap<LessonModel, Lesson>().ReverseMap();




            /*             CreateMap<Chemical, ChemicalViewModel>()
                 .ForMember(dest => dest._Id, src => src.MapFrom(x => x.Id));*/
        }
    }
}
