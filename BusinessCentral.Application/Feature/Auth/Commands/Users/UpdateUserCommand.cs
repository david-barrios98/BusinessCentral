using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
{
    public record UpdateUserCommand(UpdateUserDTO User) : IRequest<Result<bool>>;
}