using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record CreateUserCommand(CreateUserDTO User) : IRequest<Result<int?>>;
}