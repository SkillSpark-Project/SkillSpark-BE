using Application.ViewModels.AuthViewModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations.Auths
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Username).NotEmpty().WithMessage("Tên đăng nhập không được để trống.").MaximumLength(50)
                .WithMessage("Tên đăng nhập không quá 50 ký tự.").Must(IsLetterOrDigitOnly).WithMessage("Tên đăng nhập chỉ được chứa chữ hoặc số.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống.")
                .MaximumLength(50)
                .WithMessage("Email không quá 50 ký tự.").EmailAddress().WithMessage("Email không đúng định dạng.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại không được để trống.")
               .MaximumLength(10)
                .WithMessage("Số điện thoại phải có 10 ký tự.").MinimumLength(10)
                .WithMessage("Số điện thoại  phải có 10 ký tự.").Must(IsPhoneNumberValid).WithMessage("Số điện thoại chỉ được chứa các chữ số.")
                .Must(IsPhoneNumberStartWith).WithMessage("Số điện thoại chỉ được bắt đầu bằng các đầu số 03, 05, 07, 08, 09.");
            RuleFor(x => x.Fullname).NotEmpty().WithMessage("Họ tên không được để trống.")
                .MaximumLength(50)
                .WithMessage("Họ tên không quá 50 ký tự.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
              .MaximumLength(50).WithMessage("Mật khẩu không quá 50 ký tự.")
              .Must(ContainsDigit).WithMessage("Mật khẩu phải có chứa ít nhất một chữ số.")
              .Must(ContainsLowercase).WithMessage("Mật khẩu phải có chứa ít nhất một chữ cái thường.").
              Must(ContainsUppercase).WithMessage("Mật khẩu phải có chứa ít nhất một chữ cái in hoa.")
              .Must(ContainsSpecialCharacter).WithMessage("Mật khẩu phải có chứa ít nhất một ký tự đặc biệt.");
            RuleFor(x => x.Birthday).NotEmpty().WithMessage("Họ tên không được để trống.")
               .LessThan(DateTime.Now)
               .WithMessage("Ngày sing phải sau ngyaf hôm nay.");
        }
        private bool IsPhoneNumberValid(string phoneNumber)
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

        private bool IsPhoneNumberStartWith(string phoneNumber)
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

        private bool ContainsDigit(string input)
        {
            return input.Any(char.IsDigit);
        }

        private bool ContainsLowercase(string input)
        {
            return input.Any(char.IsLower);
        }
        private bool ContainsUppercase(string input)
        {
            return input.Any(char.IsUpper);
        }
        private bool ContainsSpecialCharacter(string input)
        {
            return input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
        private bool IsLetterOrDigitOnly(string input)
        {
            return input.All(char.IsLetterOrDigit);
        }
    }
}
