using Application.ViewModels.ChapterViewModels;
using FluentValidation;
namespace Application.Validations.Chapters
{
    public class ChapterModelValidator:AbstractValidator<ChapterModel>
    {
        public ChapterModelValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty().WithMessage("Id của khóa học không được để trống.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên chương không được để trống.")
          .MaximumLength(100).WithMessage("Tên chương không quá 100 ký tự.");
        }
    }
}
