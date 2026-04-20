using MediatR;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Feature.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IUserSessionRepository _sessionRepo;
        private readonly IRedisService _redisService;

        public LogoutHandler(IRefreshTokenRepository refreshRepo, IUserSessionRepository sessionRepo, IRedisService redisService)
        {
            _refreshRepo = refreshRepo;
            _sessionRepo = sessionRepo;
            _redisService = redisService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Si traes refreshToken específico, revócalo
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                var token = await _refreshRepo.GetActiveByTokenAsync(request.RefreshToken);
                RefreshToken tokenAudit = new RefreshToken();
       
                if (!string.IsNullOrEmpty(token?.RefreshToken))
                {
                    tokenAudit.Token = token.RefreshToken;
                    await _refreshRepo.RevokeAsync(tokenAudit, null);
                }
            }

            // Si pasa userId: revocamos todos los refresh tokens del usuario y cerramos sesiones
            if (request.UserId.HasValue)
            {
                await _refreshRepo.RevokeAllByUserAsync(request.UserId.Value, null);
                await _sessionRepo.CloseSessionsByCompanyAsync(request.UserId.Value);
            }

            // Si pasa companyId: revocamos tokens (si snapshot companyId existe) y cerramos sesiones
            if (request.CompanyId.HasValue)
            {
                await _refreshRepo.RevokeAllByCompanyAsync(request.CompanyId.Value, null);
                await _sessionRepo.CloseSessionsByCompanyAsync(request.CompanyId.Value);
            }

            // Si pasa sessionId: cerramos esa sesión concreta
            if (request.SessionId.HasValue)
            {
                var session = await _sessionRepo.GetByIdAsync(request.SessionId.Value);
                if (session != null && session.LogoutAt == null)
                {
                    session.LogoutAt = DateTime.UtcNow;
                    session.IsSuccess = false;
                    await _sessionRepo.UpdateAsync(session);
                }
            }

            if (!string.IsNullOrEmpty(request.Token))
            {
                await _redisService.RevokeTokenAsync(request.Token);
            }

            return Result<bool>.Success(true);
        }
    }
}