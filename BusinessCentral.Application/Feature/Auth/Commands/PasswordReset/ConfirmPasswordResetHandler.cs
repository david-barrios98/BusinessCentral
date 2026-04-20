using BusinessCentral.Application.Feature.Auth.Commands.PasswordReset;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Auth.Commands.PasswordReset
{
    public class ConfirmPasswordResetHandler : IRequestHandler<ConfirmPasswordResetCommand, Result<bool>>
    {
        private readonly IPasswordResetRepository _repo;
        private readonly IHashPasswordService _hashService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IUserSessionRepository _sessionRepo;

        public ConfirmPasswordResetHandler(
            IPasswordResetRepository repo,
            IHashPasswordService hashService,
            IRefreshTokenRepository refreshRepo,
            IUserSessionRepository sessionRepo)
        {
            _repo = repo;
            _hashService = hashService;
            _refreshRepo = refreshRepo;
            _sessionRepo = sessionRepo;
        }

        public async Task<Result<bool>> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var token = request.Request.Token;
            var newPassword = request.Request.NewPassword;

            var resetEntity = await _repo.GetActiveByTokenAsync(token:token);
            if (resetEntity == null)
            {
                return Result<bool>.Failure("Token inválido o expirado", "INVALID_TOKEN", "Unauthorized");
            }

            var userId = resetEntity.UserId;

            var newHashed = _hashService.Hash(newPassword);

            // Actualiza contraseña mediante SP
            await _repo.UpdateUserPasswordAsync(userId, newHashed);

            // Marca token como usado (SP)
            await _repo.MarkAsUsedAsync(token);

            // Revocar tokens y cerrar sesiones por seguridad
            await _refreshRepo.RevokeAllByUserAsync(userId, null);
            await _sessionRepo.CloseSessionsByUserAsync(userId);

            return Result<bool>.Success(true);
        }
    }
}