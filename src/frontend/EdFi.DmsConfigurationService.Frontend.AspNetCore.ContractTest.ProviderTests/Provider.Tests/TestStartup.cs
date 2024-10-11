// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Middleware;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;
using EdFi.DmsConfigurationService.Backend;
using EdFi.DmsConfigurationService.Backend.Keycloak;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;
using FakeItEasy;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{
    public class TestStartup
    {
        private ITokenManager? _tokenManager;
        //private IClientRepository? _clientRepository;
        public IConfiguration? Configuration { get; }

        public TestStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /* public TestStartup(IConfiguration configuration, IClientRepository clientRepository)
        {
            Configuration = configuration;
            _clientRepository = clientRepository;
        } */

        public void ConfigureServices(IServiceCollection services)
        {
            _tokenManager = A.Fake<ITokenManager>();
            var token =
                """
            {
                "access_token":"input123token",
                "expires_in":900,
                "token_type":"bearer"
            }
            """;
            A.CallTo(() => _tokenManager.GetAccessTokenAsync(A<IEnumerable<KeyValuePair<string, string>>>.Ignored))
                .Returns(Task.FromResult(token));

            // Register the fake _tokenManager as a singleton so it's used wherever ITokenManager is required
            services.AddSingleton(_tokenManager);

            // Add other services needed by your application
            services.AddControllers();

/*             var assembly = typeof(IdentityModule).Assembly;

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
*/
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DMS Configuration Service API", Version = "v1" });
            });

            services.TryAddSingleton<ITokenManager, FakeTokenManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DMS Configuration Service API v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
