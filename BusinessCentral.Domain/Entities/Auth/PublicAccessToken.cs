using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Auth;

[Table("PublicAccessToken", Schema = "auth")]
public sealed class PublicAccessToken
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty; // hex SHA256

    [Required]
    [MaxLength(30)]
    public string Scope { get; set; } = "HR_ACCOUNT"; // extensible

    public DateTime ExpiresAt { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }
}

