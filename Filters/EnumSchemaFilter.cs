using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GraduationProject.Infrastructure
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                schema.Type = "string";
                schema.Format = null;

                var enumValues = Enum.GetNames(context.Type);
                foreach (var enumValue in enumValues)
                {
                    schema.Enum.Add(new OpenApiString(enumValue));
                }
            }
        }
    }
}