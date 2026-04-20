using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.PasswordReset
{
    public record ConfirmPasswordResetCommand(PasswordResetConfirmDTO Request) : IRequest<Result<bool>>;
}