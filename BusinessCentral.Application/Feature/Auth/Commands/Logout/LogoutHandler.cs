using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Application.Features.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IUserSessionRepository _sessionRepo;

        public LogoutHandler(IRefreshTokenRepository refreshRepo, IUserSessionRepository sessionRepo)
        {
            _refreshRepo = refreshRepo;
            _sessionRepo = sessionRepo;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshRepo.GetActiveByTokenAsync(request.RefreshToken);
            if (token != null)
            {
                await _refreshRepo.RevokeAsync(token, null);
            }

            if (request.SessionId.HasValue)
            {
                var session = await _sessionRepo.GetByIdAsync(request.SessionId.Value);
                if (session != null)
                {
                    session.LogoutAt = DateTime.UtcNow;
                    await _sessionRepo.UpdateAsync(session);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}