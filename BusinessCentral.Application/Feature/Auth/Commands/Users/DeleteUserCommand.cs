using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record DeleteUserCommand(int UserId) : IRequest<Result<bool>>;
}