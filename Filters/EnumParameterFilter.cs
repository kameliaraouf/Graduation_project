using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GraduationProject.Infrastructure
{
    public class EnumParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var parameterType = context.ParameterInfo?.ParameterType;
            if (parameterType == null) return;

            if (parameterType.IsEnum)
            {
                parameter.Schema.Type = "string";
                parameter.Schema.Format = null;
                parameter.Schema.Enum.Clear();

                var enumValues = Enum.GetNames(parameterType);
                foreach (var value in enumValues)
                {
                    parameter.Schema.Enum.Add(new OpenApiString(value));
                }
            }
        }
    }
}