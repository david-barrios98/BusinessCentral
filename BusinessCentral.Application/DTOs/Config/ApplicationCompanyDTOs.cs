using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Config;

public sealed class ApplicationCompanyDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    [JsonPropertyName("applicationCode")]
    public string ApplicationCode { get; set; } = null!;

    /// <summary>email | phone | document (equivalente legacy documentNumber)</summary>
    [JsonPropertyName("loginField")]
    public string LoginField { get; set; } = null!;

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("create")]
    public DateTime? Create { get; set; }

    [JsonPropertyName("update")]
    public DateTime? Update { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }
}

public sealed class UpsertApplicationCompanyRequestDTO
{
    /// <summary>Si se envía y es &gt; 0, actualiza el registro existente de esa compañía.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("applicationCode")]
    [Required]
    [MaxLength(200)]
    public string ApplicationCode { get; set; } = null!;

    /// <summary>Valores: email, phone, document (también se acepta documentNumber y se normaliza a document).</summary>
    [JsonPropertyName("loginField")]
    [Required]
    [MaxLength(20)]
    public string LoginField { get; set; } = null!;

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}
