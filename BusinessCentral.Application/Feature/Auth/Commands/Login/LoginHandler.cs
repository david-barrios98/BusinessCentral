using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDTO>>
    {
        private readonly ILoginRepository _repository;
        private readonly IHashPasswordService _hashService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        public LoginHandler(
            ILoginRepository repository,
            IHashPasswordService hashService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository)
        {
            _repository = repository;
            _hashService = hashService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<Result<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetLoginUserAsync(request);

            if (user == null)
            {
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.USER_NOT_FOUND],
                    MessageKeys.USER_NOT_FOUND,
                    "NotFound");
            }

            if (!_hashService.Verify(request.userLogin.Password, user.Password))
            {
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.ERROR_LOGIN],
                    MessageKeys.ERROR_LOGIN,
                    "Unauthorized");
            }

            JwtUserDto jwtUser = new JwtUserDto
            {
                userId = user.UserId,
                userName = user.UserName,
                companyId = user.CompanyId.ToString(),
                companyName = user.CompanyName?.ToString() ?? string.Empty
            };

            var accessToken = _tokenService.GenerateAccessToken(jwtUser);

            // Antes de crear nuevo refresh token / sesión, inhabilitamos los previos
            await _refreshTokenRepository.RevokeAllByUserAsync(user.UserId, null);
            await _userSessionRepository.CloseSessionsByUserAsync(user.UserId);

            // Crear refresh token con snapshot
            var refreshValue = _tokenService.GenerateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CompanyId = user.CompanyId
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);

            // Registrar sesión
            var session = new UserSession
            {
                UserId = user.UserId,
                CompanyId = user.CompanyId,
                Platform = "Web",
                DeviceFingerprint = null,
                IpAddress = null,
                UserAgent = null,
                LoginAt = DateTime.UtcNow,
                IsSuccess = true
            };

            await _userSessionRepository.AddAsync(session);

            user.AccessToken = accessToken;
            user.RefreshToken = refreshValue;
            user.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();
            user.IssuedAt = DateTime.UtcNow;

            return Result<LoginResponseDTO>.Success(user);
        }
    }
}