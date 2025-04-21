using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Type = "string";
            schema.Enum = context.Type.GetEnumNames() 
                .Select(name => new OpenApiString(name))
                .Cast<IOpenApiAny>()
                .ToList();

            schema.Description = context.Type.GetCustomAttributes<DisplayAttribute>().FirstOrDefault()?.Description;
        }
    }
}