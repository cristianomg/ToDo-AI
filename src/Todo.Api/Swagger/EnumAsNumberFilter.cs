using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Todo.Api.Swagger
{
    public class EnumAsNumberFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Type = "integer";
                schema.Format = "int32";
                schema.Enum.Clear();
            }
        }
    }

}
