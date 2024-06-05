using FluentValidation.Results;

namespace Application.Commons.Exeptions
{
    public class ValidationException :Exception
    {
        public ValidationException()
       : base("Đã xảy ra một hoặc nhiều lỗi xác thực.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
