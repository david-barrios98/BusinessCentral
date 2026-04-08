using FluentValidation;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.userLogin.UserName)
                .NotEmpty();

            RuleFor(x => x.userLogin.Password)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.userLogin.CompanyId)
                .NotEmpty();
        }
    }
}