// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet.Verifier;
using FakeItEasy;
using EdFi.DmsConfigurationService.Backend;
using Microsoft.Extensions.DependencyInjection;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Configuration;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest.Provider.Tests
{
    [TestFixture]
    public class ProviderTests : IDisposable
    {
        private static readonly Uri _providerUri = new("http://localhost:5000");
        private ITokenManager? _tokenManager;

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private IHost server;
        private IPactVerifier verifier;

        public ProviderTests()
        {
            //Probably this is not needed since class FakeTokenManager was created and it is being called on TestStartup
            _tokenManager = A.Fake<ITokenManager>();
            var token =
                """
            {
                "access_token":"input123token",
                "expires_in":900,
                "token_type":"bearer"
            }
            """;
            A.CallTo(() => _tokenManager.GetAccessTokenAsync(A<IEnumerable<KeyValuePair<string, string>>>.Ignored)).Returns(Task.FromResult(token));

            this.server = Host.CreateDefaultBuilder()
                        .ConfigureServices((collection) =>
                                {
                                    collection.Configure<IdentitySettings>(opts =>
                                    {
                                        opts.AllowRegistration = false;
                                    });
                                    collection.AddTransient((x) => new TokenRequest.Validator());
                                    collection.AddTransient((x) => _tokenManager!);
                                })
                             .ConfigureWebHostDefaults(webBuilder =>
                             {
                                 webBuilder.UseEnvironment("Development");
                                 webBuilder.UseUrls(_providerUri.ToString());
                                 webBuilder.UseStartup<TestStartup>();
                                 //webBuilder.UseStartup(context => new TestStartup(context.Configuration, _clientRepository));
                             }).Build();

            this.server.Start();
            //This was used to make some time and verify the swagger during the test execution
            //for some reason it was not working when debugging
            //Thread.Sleep(180000);
            this.verifier = new PactVerifier(new PactVerifierConfig()
            {
                LogLevel = PactNet.PactLogLevel.Debug
            });
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

            //Pact 4.5
            this.verifier!.ServiceProvider("address_provider", _providerUri)
                .WithFileSource(new FileInfo(pactFile))
                //.WithProviderStateUrl(new Uri($"{PactServiceUri}provider-states"))
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
