using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Feature.Auth.Commands.Users
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<int?>>
    {
        private readonly IUserRepository _repo;
        private readonly IHashPasswordService _hash;

        public CreateUserHandler(IUserRepository repo, IHashPasswordService hash)
        {
            _repo = repo;
            _hash = hash;
        }

        public async Task<Result<int?>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;
            dto.Password = _hash.Hash(dto.Password);
            var id = await _repo.CreateUserAsync(dto);
            return Result<int?>.Success(id);
        }
    }
}