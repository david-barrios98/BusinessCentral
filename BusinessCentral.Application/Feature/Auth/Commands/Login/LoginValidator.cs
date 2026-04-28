using FluentValidation;

namespace BusinessCentral.Application.Feature.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.userLogin.UserName)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.userLogin.Password)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.userLogin.CompanyId)
                // companyId es requerido para login tenant, pero opcional para login system/web.
                // Si viene, debe tener al menos 1 char (y usualmente será un int válido).
                .MinimumLength(1)
                .When(x => !string.IsNullOrWhiteSpace(x.userLogin.CompanyId));
        }
    }
}