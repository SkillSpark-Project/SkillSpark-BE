using FluentValidation.Results;

namespace Application.Commons.Exeptions
{
    public class ValidationException :Exception
    {        public List<string> Errors { get; }

        public ValidationException(List<string> errors)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Errors);
        }
    }
}
