using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record GetUserQuery(int CompanyId, int UserId) : IRequest<Result<UserResponseDTO>>;
}