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

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{
    public class TestStartup
    {
        private readonly Startup inner;

        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

/*         public void ConfigureServices(IHost server, Uri _providerUri, IClientRepository _clientRepository)
        {
            server = Host.CreateDefaultBuilder()
                             .ConfigureWebHostDefaults(webBuilder =>
                             {
                                 webBuilder.UseUrls(_providerUri.ToString());
                                 webBuilder.ConfigureServices((collection) =>
                                 {
                                     collection.AddTransient((x) => new RegisterRequest.Validator(_clientRepository!));
                                     collection.AddTransient((x) => _clientRepository!);
                                 });
                             }).Build();

        }*/

        public void ConfigureServices(IServiceCollection services)
        {
            IClientRepository? _clientRepository = A.Fake<IClientRepository>();
            // Register the Validator and the _clientRepository in the DI container.
            services.AddTransient((x) => new RegisterRequest.Validator(_clientRepository!));
            services.AddTransient((x) => _clientRepository!);

            // Register other services (e.g., a fake repository or other dependencies).
            //services.AddSingleton<IClientRepository, FakeClientRepository>();

            // Call inner configuration if needed.
            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
