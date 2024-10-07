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
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{


    public class ProviderApiTests : IDisposable
    {
        private string _providerUri { get; }
        private string _pactServiceUri { get; }
        private IWebHost _webHost { get; }

        public ProviderApiTests()
        {
            _providerUri = "http://localhost:9000";
            _pactServiceUri = "http://localhost:9001";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        [Test]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {

                // NOTE: We default to using a ConsoleOutput,
                // however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput>
                            {
                                new ConsoleOutput()
                            },

                // Output verbose verification logs to the test output
                LogLevel = PactLogLevel.Debug,
            };

            //IPactVerifier pactVerifier = new PactVerifier(config);
            //string pactUrl = System.Environment.GetEnvironmentVariable("PACT_URL");
            string pactUrl = "http://localhost:5000";

            //string pactFile = System.Environment.GetEnvironmentVariable("PACT_FILE");z
            string pactFile = Path.Combine("..",
                                                   "..",
                                                   "..",
                                                   "..",
                                                   "EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.ConsumerTests",
                                                   "pacts",
                                                   "DMS API Consumer-DMS Configuration Service API.json");

            string? providerName = !String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("PACT_PROVIDER_NAME"))
                                    ? System.Environment.GetEnvironmentVariable("PACT_PROVIDER_NAME")
                                    : "DMS Configuration Service API";
            //string version = Environment.GetEnvironmentVariable("GIT_COMMIT");
            //string branch = Environment.GetEnvironmentVariable("GIT_BRANCH");
            //string buildUri = $"{Environment.GetEnvironmentVariable("GITHUB_SERVER_URL")}/{Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")}/actions/runs/{Environment.GetEnvironmentVariable("GITHUB_RUN_ID")}";


            if (pactFile != "" && pactFile != null)
            // Verify a local file, provided by PACT_FILE, verification results are never published
            // This step does not require a Pact Broker
            {
                //pactVerifier.ServiceProvider(providerName, new Uri(_providerUri))
                ////.WithFileSource(new FileInfo(pactUrl))
                //.WithFileSource(new FileInfo(pactFile))
                //.WithProviderStateUrl(new Uri($"{_pactServiceUri}/provider-states"))
                //.Verify();
            }
            else if (pactUrl != "" && pactUrl != null)
            // Verify a remote file fetched from a pact broker, provided by PACT_URL, verification results may be published
            // This step requires a Pact Broker
            {
                //pactVerifier.ServiceProvider(providerName, new Uri(_providerUri))
                //.WithUriSource(new Uri(pactUrl), options =>
                //{
                //    if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN")))
                //    {
                //        options.TokenAuthentication(System.Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN"));
                //    }
                //    else if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("PACT_BROKER_USERNAME")))
                //    {
                //        options.BasicAuthentication(System.Environment.GetEnvironmentVariable("PACT_BROKER_USERNAME"), System.Environment.GetEnvironmentVariable("PACT_BROKER_PASSWORD"));
                //    }
                //    /* options.PublishResults(!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("PACT_BROKER_PUBLISH_VERIFICATION_RESULTS")), version, results =>
                //        {
                //            results.ProviderBranch(branch)
                //            .BuildUri(new Uri(buildUri));
                //        }); */
                //})
                //.WithProviderStateUrl(new Uri($"{_pactServiceUri}/provider-states"))
                //.Verify();
            }
            else
            {
                // Verify remote pacts, provided by querying the Pact Broker via consumer version selectors, verification results may be published
                // This step requires a Pact Broker
            }



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
                _webHost.Dispose();
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



        /*     public class ProviderTests : IDisposable
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
                                                   "DMS API Consumer-DMS Configuration Service API.json"); */

        /*             verifier
                        .ServiceProvider("Orders API", new Uri(_providerUri, "/provider-states")) // Define the provider (API)
                        .HonoursPactWith("Orders Consumer")         // Define the consumer
                        .PactUri(pactPath)  // The path to the Pact contract file
                        .WithHttpEndpoint(_providerUri)     // The actual HTTP endpoint for verification
                        .Verify(); */


        /*             //Act / Assert
                    verifier
                        .ServiceProvider("DMS Configuration API", _providerUri)
                        .WithFileSource(new FileInfo(pactPath))
                        .WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                        .WithRequestTimeout(TimeSpan.FromSeconds(2))
                        .WithSslVerificationDisabled()
                        .Verify();
                } */
        //}
    }
}
