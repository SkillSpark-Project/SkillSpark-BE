using Application.ViewModels.MentorViewModels.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations.Mentors
{
    public class MentorModelValidator :AbstractValidator<MentorModel>
    {
        public MentorModelValidator()
        {
            RuleFor(x => x.Introduction).NotEmpty().WithMessage("Giới thiệu bản thân không được để trống.")
             .MaximumLength(5000).WithMessage("Giới thiệu bản thân không quá 5000 ký tự.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Mô tả ngắn không được để trống.")
             .MaximumLength(500).WithMessage("Mô tả không quá 500 ký tự.");
        }
    }
}
