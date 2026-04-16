using MediatR;
using BusinessCentral.Application.Common.Results;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
{
    public record DeleteUserCommand(int UserId) : IRequest<Result<bool>>;
}