using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUserRepository _repo;

        public DeleteUserHandler(IUserRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetUserByIdAsync(request.UserId);
            if (existing == null || existing.CompanyId != request.CompanyId)
                return Result<bool>.Failure("User not found", "USER_NOT_FOUND", "NotFound");

            await _repo.DeleteUserAsync(request.CompanyId, request.UserId);
            return Result<bool>.Success(true);
        }
    }
}