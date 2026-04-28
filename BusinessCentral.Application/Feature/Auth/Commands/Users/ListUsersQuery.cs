using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record ListUsersQuery(int CompanyId, int Page, int PageSize) : IRequest<Result<PagedResult<UserResponseDTO>>>;
}