using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
{
    public record ListUsersQuery(int CompanyId, int Page, int PageSize) : IRequest<Result<List<UserResponseDTO>>>;
}