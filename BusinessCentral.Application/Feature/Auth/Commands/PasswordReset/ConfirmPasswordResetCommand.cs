using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.PasswordReset
{
    public record ConfirmPasswordResetCommand(PasswordResetConfirmDTO Request) : IRequest<Result<bool>>;
}