using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, Result<UserResponseDTO>>
    {
        private readonly IUserRepository _repo;

        public GetUserHandler(IUserRepository repo) => _repo = repo;

        public async Task<Result<UserResponseDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetUserByIdAsync(request.UserId);
            if (user == null)
                return Result<UserResponseDTO>.Failure("User not found", "USER_NOT_FOUND", "NotFound");
            return Result<UserResponseDTO>.Success(user);
        }
    }
}