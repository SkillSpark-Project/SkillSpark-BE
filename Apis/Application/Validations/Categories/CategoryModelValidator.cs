using Application.ViewModels.CategoryViewModels.Requests;

using FluentValidation;

namespace Application.Validations.Categories
{
    public class CategoryModelValidator :AbstractValidator<CategoryModel>
    {
        public CategoryModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên danh mục không được để trống.")
           .MaximumLength(100).WithMessage("Tên danh mục không quá 100 ký tự.");
        }
    }
}
