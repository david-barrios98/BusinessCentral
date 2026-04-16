using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
{
    public class ListUsersHandler : IRequestHandler<ListUsersQuery, Result<List<UserResponseDTO>>>
    {
        private readonly IUserRepository _repo;

        public ListUsersHandler(IUserRepository repo) => _repo = repo;

        public async Task<Result<List<UserResponseDTO>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListUsersAsync(request.CompanyId, request.Page, request.PageSize);
            return Result<List<UserResponseDTO>>.Success(list);
        }
    }
}