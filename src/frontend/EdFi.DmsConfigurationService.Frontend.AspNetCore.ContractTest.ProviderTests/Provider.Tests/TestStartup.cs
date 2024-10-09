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

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{
    public class TestStartup
    {
        private readonly Startup inner;

        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IOrderRepository, FakeOrderRepository>();
            //services.AddSingleton<>(IClientRepository, ClientRepository);
            this.inner.ConfigureServices(services);

            //webApplicationBuilder.Services.AddTransient<IClientRepository, ClientRepository>();
            //webApplicationBuilder.Services.AddTransient<ITokenManager, TokenManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
