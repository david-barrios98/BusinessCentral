using MediatR;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
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