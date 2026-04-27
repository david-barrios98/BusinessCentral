using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using BusinessCentral.Application.DTOs.Auth;
using System.Text.Json.Nodes;

namespace BusinessCentral.Api.Filters
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginRequestDTO))
            {
                if (schema is OpenApiSchema s)
                {
                    s.Example = new JsonObject
                    {
                        ["UserName"] = JsonValue.Create("3100000000"),
                        ["Password"] = JsonValue.Create("cc1"),
                        ["companyId"] = JsonValue.Create("1"),
                    };
                }
            }
        }
    }
}