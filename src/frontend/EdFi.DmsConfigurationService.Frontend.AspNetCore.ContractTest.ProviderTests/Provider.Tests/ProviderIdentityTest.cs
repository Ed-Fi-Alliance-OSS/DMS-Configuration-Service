// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
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
        private readonly IPactVerifier verifier;

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

            this.verifier = new PactVerifier("DMS API");
        }

        [Test]
        public void Verify()
        {
            string pactFile = Path.Combine("..",
                                                   "..",
                                                   "..",
                                                   "..",
                                                   "EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.ConsumerTests",
                                                   "pacts",
                                                   "DMS API Consumer-DMS Configuration Service API.json");

            // Act & Assert: Verify the provider against the Pact file
            this.verifier.WithHttpEndpoint(_providerUri)
                .WithFileSource(new FileInfo(pactFile))
                //.WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                .Verify();
        }

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                server.Dispose();
            }

            _disposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
