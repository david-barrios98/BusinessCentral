using BusinessCentral.Domain.Entities.Auth;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Audit
{
    [Table("PasswordResetToken", Schema = "audit")]
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersInfo User { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Token { get; set; } = null!;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }

        public bool IsActive => UsedAt == null && DateTime.UtcNow < ExpiresAt;
    }
}