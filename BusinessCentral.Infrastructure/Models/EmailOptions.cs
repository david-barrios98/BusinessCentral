namespace BusinessCentral.Infrastructure.Models
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = "localhost";
        public int SmtpPort { get; set; } = 25;
        public bool UseSsl { get; set; } = false;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string From { get; set; } = "no-reply@example.com";
        public string FrontendUrl { get; set; } = "https://app.example.com"; // usado para construir link de reset
        public string Name { get; set; } = "Tu Nombre";
    }
}