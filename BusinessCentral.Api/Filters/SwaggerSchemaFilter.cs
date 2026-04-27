using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Api.Filters;

/// <summary>
/// En Microsoft.OpenApi 2.x, <see cref="OpenApiSchema.Example"/> es <c>JsonNode</c> (no IOpenApiAny).
/// </summary>
public sealed class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(LoginRequestDTO))
            return;

        if (schema is not OpenApiSchema s)
            return;

        s.Example = new JsonObject
        {
            ["UserName"] = JsonValue.Create("3100000000"),
            ["Password"] = JsonValue.Create("cc1"),
            ["companyId"] = JsonValue.Create("1"),
        };
    }
}
