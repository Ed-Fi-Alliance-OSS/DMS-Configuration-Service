// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{
    public class ProviderTests : IDisposable
    {
        private static readonly Uri _providerUri = new("http://localhost:5000");

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IHost server;
        private readonly PactVerifier verifier;

        public ProviderTests()
        {
            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(_providerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            this.server.Start();

            this.verifier = new PactVerifier();
        }

        public void Dispose()
        {
            this.server.Dispose();
            this.verifier.Dispose();
        }

        [Test]
        public void Verify()
        {
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.ConsumerTests",
                                           "pacts",
                                           "DMS API Consumer-DMS Configuration Service API.json");

/*             verifier
                .ServiceProvider("Orders API", new Uri(_providerUri, "/provider-states")) // Define the provider (API)
                .HonoursPactWith("Orders Consumer")         // Define the consumer
                .PactUri(pactPath)  // The path to the Pact contract file
                .WithHttpEndpoint(_providerUri)     // The actual HTTP endpoint for verification
                .Verify(); */


            //Act / Assert
            verifier
                .ServiceProvider("DMS Configuration API", _providerUri)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                .WithRequestTimeout(TimeSpan.FromSeconds(2))
                .WithSslVerificationDisabled()
                .Verify();
        }
    }
}
