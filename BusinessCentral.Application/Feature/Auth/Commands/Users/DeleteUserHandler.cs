using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUserRepository _repo;

        public DeleteUserHandler(IUserRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _repo.DeleteUserAsync(request.UserId);
            return Result<bool>.Success(true);
        }
    }
}