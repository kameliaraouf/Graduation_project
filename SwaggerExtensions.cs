using GraduationProject.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraduationProject.Configuration
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerWithEnumDropdowns(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Graduation Project API", Version = "v1" });
               

                // Add custom schema IDs to resolve the conflict
                c.CustomSchemaIds(type =>
                {
                    // Add namespace prefix to enum types to avoid collision
                    if (type == typeof(GraduationProject.Data.DTO.ProductCategoryDto))
                        return "DTOProductCategory";

                    if (type == typeof(GraduationProject.Data.Entities.ProductCategory))
                        return "EntityProductCategory";

                    // For all other types, use the default behavior
                    return type.Name;
                });
                // Add enum support
                c.SchemaFilter<EnumSchemaFilter>();
                c.ParameterFilter<EnumParameterFilter>();

                // Enable annotations
                c.EnableAnnotations();

                // Add JWT authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}