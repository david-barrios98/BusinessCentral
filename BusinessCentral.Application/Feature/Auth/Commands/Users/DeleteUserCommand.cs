using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record DeleteUserCommand(int CompanyId, int UserId) : IRequest<Result<bool>>;
}