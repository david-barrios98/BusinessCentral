using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Features.Auth.Commands.Users
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
            var dto = request.User;
            if (!string.IsNullOrEmpty(dto.Password))
                dto.Password = _hash.Hash(dto.Password);

            await _repo.UpdateUserAsync(dto);
            return Result<bool>.Success(true);
        }
    }
}