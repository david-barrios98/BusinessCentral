using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;
using BusinessCentral.Domain.Entities.Audit;
using MediatR;
using System.Text.RegularExpressions;

namespace BusinessCentral.Application.Feature.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDTO>>
    {
        private readonly ILoginRepository _repository;
        private readonly IHashPasswordService _hashService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IFailedLoginAttemptService _failedLoginAttemptService;
        private readonly ICompanyModuleRepository _companyModuleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public LoginHandler(
            ILoginRepository repository,
            IHashPasswordService hashService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository,
            IFailedLoginAttemptService failedLoginAttemptService,
            ICompanyModuleRepository companyModuleRepository,
            IRolePermissionRepository rolePermissionRepository)
        {
            _repository = repository;
            _hashService = hashService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
            _failedLoginAttemptService = failedLoginAttemptService;
            _companyModuleRepository = companyModuleRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<Result<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var username = request.userLogin.UserName;
            string loginFieldType = "email"; // Valor por defecto

            if (Regex.IsMatch(username, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                loginFieldType = "email";
            }
            else if (Regex.IsMatch(username, @"^3[0-9]{9}$"))
            {
                loginFieldType = "phone";
            }
            else if (Regex.IsMatch(username, @"^[0-9]{5,12}$"))
            {
                loginFieldType = "document";
            }

            // 0. Verificar si la cuenta está bloqueada por varios intentos fallidos
            if (await _failedLoginAttemptService.IsAccountLockedAsync(username))
            {
                return Result<LoginResponseDTO>.Failure(
                    "Cuenta bloqueada por múltiples intentos fallidos. Intenta más tarde.",
                    "ACCOUNT_LOCKED",
                    "Unauthorized");
            }

            LoginResponseDTO? user = null;
            if (!string.IsNullOrWhiteSpace(request.userLogin.CompanyId))
            {
                user = await _repository.GetLoginUserAsync(request);
            }
            else
            {
                // Login de sistema (web superusuario): no usa companyId
                user = await _repository.GetSystemLoginUserAsync(request.userLogin.UserName);
            }

            // 1. Validamos existencia del usuario
            if (user == null)
            {
                // Registramos intento fallido por seguridad/limitación de intentos
                await _failedLoginAttemptService.RecordFailedAttemptAsync(username);

                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.USER_NOT_FOUND],
                    MessageKeys.USER_NOT_FOUND,
                    "NotFound");
            }

            // 2. Validamos contraseña
            if (!_hashService.Verify(request.userLogin.Password, user.Password))
            {
                // Registrar intento fallido
                await _failedLoginAttemptService.RecordFailedAttemptAsync(username);

                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.ERROR_LOGIN],
                    MessageKeys.ERROR_LOGIN,
                    "Unauthorized");
            }

            // 3. Login exitoso: limpiar contador de intentos fallidos
            await _failedLoginAttemptService.ClearFailedAttemptsAsync(username);

            // 4. Generación de Token (igual que antes)
            // Cargar permisos y módulos asignados (para UI y claims)
            user.Permissions = await _rolePermissionRepository.ListRolePermissionsAsync(user.RoleId);
            user.Modules = await _companyModuleRepository.ListCompanyModulesAsync(user.CompanyId);

            JwtUserDto jwtUser = new JwtUserDto
            {
                userId = user.UserId,
                userName = user.UserName,
                companyId = user.CompanyId.ToString(),
                companyName = user.CompanyName?.ToString() ?? string.Empty,
                LoginField = loginFieldType,
                role = user.RoleName ?? string.Empty,
                isSystemRole = user.IsSystemRole,
                isSuperUser = user.IsSuperUser,
                permissions = user.Permissions.Select(p => $"{p.ModuleCode}.{p.PermissionCode}").ToList(),
                modules = user.Modules.Where(m => m.IsEnabled).Select(m => m.ModuleCode ?? string.Empty).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
            };

            var accessToken = _tokenService.GenerateAccessToken(jwtUser);

            // Antes de crear nuevo refresh token / sesión, inhabilitamos los previos
            await _refreshTokenRepository.RevokeAllByUserAsync(user.UserId, null);
            await _userSessionRepository.CloseSessionsByUserAsync(user.UserId);

            // Registrar sesión
            var session = new UserSession
            {
                UserId = user.UserId,
                LoginField = loginFieldType,
                CompanyId = user.CompanyId,
                Platform = request.Platform,
                DeviceFingerprint = null,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                IsSuccess = true
            };

            var userSessionId = await _userSessionRepository.AddAsync(session);

            // Crear refresh token con snapshot
            var refreshValue = _tokenService.GenerateRefreshToken();
            var refreshEntity = new RefreshToken
            {
                UserSessionId = userSessionId,
                Token = refreshValue
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);


            user.UserSessionId = userSessionId;
            user.LoginField = loginFieldType;
            user.AccessToken = accessToken;
            user.RefreshToken = refreshValue;
            user.ExpiresIn = _tokenService.GetAccessTokenExpirationSeconds();
            user.IssuedAt = DateTime.UtcNow;

            return Result<LoginResponseDTO>.Success(user);
        }
    }
}