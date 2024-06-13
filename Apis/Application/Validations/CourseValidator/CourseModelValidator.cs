using Application.ViewModels.CourseViewModels.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations.CourseValidator
{
    public class CourseModelValidator : AbstractValidator<CourseModel>
    {
        public CourseModelValidator()
        {
            RuleFor(course => course.CategoryId)
                .NotEmpty().WithMessage("Danh mục không thể để trống.");

            RuleFor(course => course.Name)
                .NotEmpty().WithMessage("Tên khóa học không thể để trống.")
                .MaximumLength(100).WithMessage("Tên khóa học không quá 100 ký tự.");

            RuleFor(course => course.Image)
                .NotNull().WithMessage("Hình ảnh khóa học không thể để trống.");

            RuleFor(course => course.Description).NotEmpty().WithMessage("Mô tả khóa học không thể để trống.")
                .MaximumLength(2000).WithMessage("Mô tả khóa học không quá 2000 ký tự.");

            RuleFor(course => course.ShortDescripton).NotEmpty().WithMessage("Mô tả ngắn không thể để trống.")
                .MaximumLength(200).WithMessage("Mô tả ngắn không quá 200 ký tự.");

            RuleFor(course => course.Requirements)
                .NotEmpty().WithMessage("Yêu cầu của khóa học không thể để trống.")
                .Must(requirements => requirements.Count > 0).WithMessage("Nội dung của khóa học không thể để trống.")
                .ForEach(requirement =>
                requirement.MaximumLength(200).WithMessage("Mỗi yêu cầu của khóa học không quá 200 ký tự.")
            );

            RuleFor(course => course.Contents)
                .NotEmpty().WithMessage("Yêu cầu của khóa học không thể để trống.")
                .Must(contents => contents.Count > 0).WithMessage("Nội dung của khóa học không thể để trống.")
                .ForEach(content =>
                content.MaximumLength(200).WithMessage("Mỗi nội dung của khóa học không quá 200 ký tự.")
            );
        }
    }
}
