using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Api.Filters
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginRequestDTO))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["username"] = new Microsoft.OpenApi.Any.OpenApiString("3100000000"),
                    ["password"] = new Microsoft.OpenApi.Any.OpenApiString("cc1"),
                    ["companyId"] = new Microsoft.OpenApi.Any.OpenApiString("1")
                };
            }
        }
    }
}