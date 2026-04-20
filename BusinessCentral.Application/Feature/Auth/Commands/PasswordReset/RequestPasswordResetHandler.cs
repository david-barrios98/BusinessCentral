using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System.Security.Cryptography;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.PasswordReset
{
    public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Result<bool>>
    {
        private readonly IPasswordResetRepository _repo;
        private readonly IEmailService _emailService;

        public RequestPasswordResetHandler(IPasswordResetRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public async Task<Result<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var email = request.Request.Email.Trim().ToLowerInvariant();
            var companyId = request.Request.CompanyId;

            var user = await _repo.GetUserByEmailAndCompanyAsync(email, companyId);


            var tokenActive = await _repo.GetActiveByTokenAsync(userId: user.UserId);
            if (tokenActive != null)
            {
                return Result<bool>.Failure($"El usuario {user.LastName} ya tiene un token activo", "INVALID_TOKEN", "Unauthorized");
            }

            // Responder siempre OK para evitar enumarción de usuarios
            if (user == null)
                return Result<bool>.Success(true);

            // Generar token URL-safe
            var tokenBytes = RandomNumberGenerator.GetBytes(48);
            var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');

            await _repo.InsertPasswordResetTokenAsync(user.UserId, token);

            // Construir link de frontend (lee config en Email service)
            var resetUrl = $"{/* frontend base configurable */ "https://app.example.com"}/auth/reset-password?token={Uri.EscapeDataString(token)}";

            var subject = "Recuperación de contraseña";
            var body = $@"
                <p>Hola {user.FirstName ?? user.UserName},</p>
                <p>Solicitaste restablecer contraseña. Haz clic en el enlace válido por 2 horas UTC:</p>
                <p><a href=""{resetUrl}"">{resetUrl}</a></p>";

            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Result<bool>.Success(true);
        }
    }
}