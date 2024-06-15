using Application.ViewModels.UserViewModels.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations.Users
{
    public class UserModelValidator : AbstractValidator<UserRequestModel>
    {
        public UserModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
                .MaximumLength(50)
                .WithMessage("Tên đăng nhập không quá 50 ký tự.").Must(IsLetterOrDigitOnly);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại không được để trống.")
               .MaximumLength(10)
                .WithMessage("Số điện thoại phải có 10 ký tự.").MinimumLength(10)
                .WithMessage("Số điện thoại  phải có 10 ký tự.").Must(IsPhoneNumberValid).WithMessage("Số điện thoại chỉ được chứa các chữ số.")
                .Must(IsPhoneNumberStartWith).WithMessage("Số điện thoại chỉ được bắt đầu bằng các đầu số 03, 05, 07, 08, 09.");
            RuleFor(x => x.Fullname).NotEmpty().WithMessage("Họ tên không được để trống.")
                .MaximumLength(50)
                .WithMessage("Họ tên ngắn không quá 50 ký tự.");
            RuleFor(x => x.Birthday).NotEmpty().WithMessage("Họ tên không được để trống.")
               .LessThan(DateTime.Today)
               .WithMessage("Ngày sing phải trước ngày hôm nay hôm nay.");
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            foreach (char c in phoneNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPhoneNumberStartWith(string phoneNumber)
        {
            if (phoneNumber.StartsWith("08") || phoneNumber.StartsWith("09") || phoneNumber.StartsWith("03") || phoneNumber.StartsWith("07") || phoneNumber.StartsWith("05"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ContainsDigit(string input)
        {
            return input.Any(char.IsDigit);
        }

        public bool ContainsLowercase(string input)
        {
            return input.Any(char.IsLower);
        }
        public bool ContainsUppercase(string input)
        {
            return input.Any(char.IsUpper);
        }
        public bool ContainsSpecialCharacter(string input)
        {
            return input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
        public bool IsLetterOrDigitOnly(string input)
        {
            return input.All(char.IsLetterOrDigit);
        }
    }
}
