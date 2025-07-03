using GraduationProject.Data.Models;
using GraduationProject.Data.Repositories;
using GraduationProject.Repositories;
using GraduationProject.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GraduationProject.Extensions
{
    // Make sure this is a non-generic static class at the root level
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all the chat bot related services with the DI container
        /// </summary>
        public static IServiceCollection AddChatBotServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the repository
            services.AddScoped<IChatBotRepository, ChatBotRepository>();

            // Register the services
            services.AddScoped<IChatBotService, ChatBotService>();

            // Register the chat session service as a singleton so it persists across requests
            services.AddSingleton<IChatSessionService, ChatSessionService>();

            // Configure ChatBot settings
            services.Configure<ChatBotSettings>(configuration.GetSection("ChatBotSettings"));

            // Register HttpClient for API calls
            services.AddHttpClient<IChatBotService, ChatBotService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}