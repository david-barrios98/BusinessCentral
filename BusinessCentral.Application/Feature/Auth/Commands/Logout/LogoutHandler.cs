using System.Security.Claims;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Auth.Commands.Logout;

public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IUserSessionRepository _userSessions;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ITokenService _tokenService;

    public LogoutHandler(IUserSessionRepository userSessions, IRefreshTokenRepository refreshTokens, ITokenService tokenService)
    {
        _userSessions = userSessions;
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 1) Resolver userId (prioridad: explùcito -> access token -> refresh token)
        int? userId = request.UserId;

        if (userId == null && !string.IsNullOrWhiteSpace(request.Token))
        {
            if (_tokenService.TryValidateToken(request.Token, out var principal) && principal != null)
            {
                var claim = principal.FindFirst("userId")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(claim, out var parsed))
                    userId = parsed;
            }
        }

        long? sessionId = request.SessionId;

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var snapshot = await _refreshTokens.GetActiveByTokenAsync(request.RefreshToken);
            if (snapshot != null)
            {
                sessionId ??= snapshot.UserSessionId;
                userId ??= snapshot.UserId;
            }
        }

        if (userId == null || userId <= 0)
            return Result<bool>.Failure("No se pudo resolver el usuario para cerrar sesiùn.", "LOGOUT_USER_REQUIRED", "BadRequest");

        // 2) Revocar refresh tokens
        // - Si tenemos SessionId (resuelto por refreshToken) revocamos por sesiÛn.
        // - Si no, revocamos por UserId (todas las sesiones del usuario).
        if (sessionId != null && sessionId > 0)
            await _refreshTokens.RevokeAllByUserAsync(sessionId.Value, replacedByToken: null);
        else
            await _refreshTokens.RevokeAllByUserIdAsync(userId.Value, replacedByToken: null);

        // 3) Cerrar sesiones (marca LogoutAt + IsSuccess=0 segùn SP)
        await _userSessions.CloseSessionsByUserAsync(userId.Value);

        return Result<bool>.Success(true);
    }
}