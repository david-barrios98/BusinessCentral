using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BusinessCentral.Api.Filters;

/// <summary>
/// Permite que Swagger genere correctamente endpoints con [FromForm] + IFormFile / List&lt;IFormFile&gt;.
/// </summary>
public sealed class SwaggerFileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Source?.Id?.Equals("Form", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (!formParams.Any())
            return;

        var hasFile = formParams.Any(p =>
            p.Type == typeof(IFormFile) ||
            (p.Type.IsGenericType &&
             p.Type.GetGenericTypeDefinition() == typeof(List<>) &&
             p.Type.GenericTypeArguments[0] == typeof(IFormFile)));

        if (!hasFile)
            return;

        var requestBody = operation.RequestBody as OpenApiRequestBody ?? new OpenApiRequestBody();
        requestBody.Required = true;

        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>(),
            Required = new HashSet<string>()
        };

        foreach (var p in formParams)
        {
            var name = p.Name;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            OpenApiSchema propSchema;

            if (p.Type == typeof(IFormFile))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" };
            }
            else if (p.Type.IsGenericType &&
                     p.Type.GetGenericTypeDefinition() == typeof(List<>) &&
                     p.Type.GenericTypeArguments[0] == typeof(IFormFile))
            {
                propSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" }
                };
            }
            else if (p.Type == typeof(string))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.String };
            }
            else if (p.Type == typeof(int) || p.Type == typeof(int?))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" };
            }
            else if (p.Type == typeof(long) || p.Type == typeof(long?))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" };
            }
            else if (p.Type == typeof(bool) || p.Type == typeof(bool?))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.Boolean };
            }
            else if (p.Type == typeof(DateTime) || p.Type == typeof(DateTime?))
            {
                propSchema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" };
            }
            else
            {
                // fallback: string
                propSchema = new OpenApiSchema { Type = JsonSchemaType.String };
            }

            schema.Properties[name] = propSchema;

            if (p.IsRequired)
                schema.Required.Add(name);
        }

        if (requestBody.Content is null)
            return;

        requestBody.Content["multipart/form-data"] = new OpenApiMediaType { Schema = schema };
        operation.RequestBody = requestBody;
    }
}

