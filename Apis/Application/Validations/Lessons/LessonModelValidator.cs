using Application.ViewModels.LessonViewModels.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations.Lessons
{
    public class LessonModelValidator : AbstractValidator<LessonModel>
    {
        public LessonModelValidator()
        {
            RuleFor(x => x.ChapterId)
               .NotEmpty().WithMessage("ChapterId không được để trống");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên bài học không thể để trống.")
                .MaximumLength(100).WithMessage("Tên bài học không quá 100 ký tự.");

            RuleFor(x => x.VideoFile)
                .NotNull().WithMessage("Video bài giảng không được để trống.")
                .Must(BeAValidVideo).WithMessage("Video bài giảng không đúng định dạng.")
                .Must(BeWithinAllowedSize).WithMessage("Video bài giảng không được phép vượt quá giới hạn cho phép (512 MB).");

            RuleForEach(x => x.Documents)
           .Must(BeAValidDocument).WithMessage("Tài liệu không đúng định dạng.")
           .Must(DocumentBeWithinAllowedSize).WithMessage("Tài liệu không được phép vượt quá giới hạn cho phép (200 MB).");
        }

        private bool BeAValidVideo(IFormFile file)
        {
            var allowedExtensions = new[] { ".mp4", ".avi", ".mkv" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool BeWithinAllowedSize(IFormFile file)
        {
            const long maxAllowedSize = 512 * 1024 * 1024; // 100 MB
            return file.Length <= maxAllowedSize;
        }

        private bool BeAValidDocument(IFormFile file)
        {
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool DocumentBeWithinAllowedSize(IFormFile file)
        {
            const long maxDocumentSize = 200 * 1024 * 1024; // 10 MB
            return file.Length <= maxDocumentSize;
        }
    }
}