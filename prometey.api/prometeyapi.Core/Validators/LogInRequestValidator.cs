using FluentValidation;
using prometeyapi.Core.DTOs.AuthDTOs.Request;

namespace prometeyapi.Core.Validators;

public class LogInRequestValidator : AbstractValidator<LogInRequestDTO>
{
    public LogInRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}