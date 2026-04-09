using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.Constants;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Core.Application.DTOs;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDTO>>
    {
        private readonly ILoginRepository _repository;
        private readonly IHashPasswordService _hashService;
        private readonly ITokenService _tokenService;

        public LoginHandler(
            ILoginRepository repository,
            IHashPasswordService hashService,
            ITokenService tokenService)
        {
            _repository = repository;
            _hashService = hashService;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetLoginUserAsync(request);

            // 1. Validamos existencia del usuario
            if (user == null)
            {
                // Usamos "NotFound" para que el Controller lance un 404
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.USER_NOT_FOUND],
                    MessageKeys.USER_NOT_FOUND,
                    "NotFound");
            }

            // 2. Validamos contraseña
            if (!_hashService.Verify(request.userLogin.Password, user.Password))
            {
                // Usamos "Unauthorized" para que el Controller lance un 401
                return Result<LoginResponseDTO>.Failure(
                    Messages.GENERAL[MessageKeys.ERROR_LOGIN],
                    MessageKeys.ERROR_LOGIN,
                    "Unauthorized");
            }

            // 3. Generación de Token
            JwtUserDto jwtUser = new JwtUserDto
            {
                userId = user.UserId,
                userName = user.UserName,
                companyId = user.CompanyId.ToString(),
                companyName = user.CompanyName.ToString()
            };

            var token = _tokenService.GenerateAccessToken(jwtUser);

            // Asignamos el token al DTO de respuesta
            user.AccessToken = token;

            return Result<LoginResponseDTO>.Success(user);
        }
    }
}