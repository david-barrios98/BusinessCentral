using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public record ListUsersQuery(int CompanyId, int Page, int PageSize) : IRequest<Result<List<UserResponseDTO>>>;
}