using FluentValidation;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.username)
                .NotEmpty();

            RuleFor(x => x.password)
                .NotEmpty()
                .MinimumLength(2);
        }
    }
}