using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Config;

public sealed class UpsertModuleRequestDTO
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("code")]
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [MaxLength(250)]
    public string? Description { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public sealed class UpsertBusinessNatureRequestDTO
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("code")]
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public sealed class UpsertFacilityTypeRequestDTO
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public sealed class UpsertPaymentMethodRequestDTO
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("code")]
    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("appliesTo")]
    [MaxLength(20)]
    public string AppliesTo { get; set; } = "ANY";

    [JsonPropertyName("description")]
    [MaxLength(300)]
    public string? Description { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

public sealed class UpsertFulfillmentMethodRequestDTO
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("code")]
    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("appliesTo")]
    [MaxLength(20)]
    public string AppliesTo { get; set; } = "ANY";

    [JsonPropertyName("description")]
    [MaxLength(300)]
    public string? Description { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

