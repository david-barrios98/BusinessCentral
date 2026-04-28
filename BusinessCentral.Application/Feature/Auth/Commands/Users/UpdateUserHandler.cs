using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        private readonly IUserRepository _repo;
        private readonly IHashPasswordService _hash;

        public UpdateUserHandler(IUserRepository repo, IHashPasswordService hash)
        {
            _repo = repo;
            _hash = hash;
        }

        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetUserByIdAsync(request.User.UserId);
            if (existing == null || existing.CompanyId != request.CompanyId)
                return Result<bool>.Failure("User not found", "USER_NOT_FOUND", "NotFound");

            var dto = request.User;
            if (!string.IsNullOrEmpty(dto.Password))
                dto.Password = _hash.Hash(dto.Password);

            await _repo.UpdateUserAsync(request.CompanyId, dto);
            return Result<bool>.Success(true);
        }
    }
}