// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Middleware;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PactNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using PactNet.Infrastructure.Outputters;
using System.Text.Json;
using System.Text.Json.Serialization;
using Moq;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;
using EdFi.DmsConfigurationService.Backend;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Configuration;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

[TestFixture]
public class ConsumerIdentityTest
{
    private readonly IPactBuilderV3 pact;
    //private RequestDelegate _next;
    //private ILogger<RequestLoggingMiddleware> _logger;

    //private readonly Mock<IHttpClientFactory> mockFactory;

    private ITokenManager? _tokenManager;

    [SetUp]
    public void Setup()
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
        A.CallTo(() => _tokenManager.GetAccessTokenAsync(A<IEnumerable<KeyValuePair<string, string>>>.Ignored)).Returns(Task.FromResult(token));
    }

    public ConsumerIdentityTest()
    {
        //this.mockFactory = new Mock<IHttpClientFactory>();

        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new List<IOutput> { new ConsoleOutput() },
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            },
            LogLevel = PactLogLevel.Debug
        };

        IPactV3 pact = Pact.V3("DMS API Consumer", "DMS Configuration Service API", config);
        this.pact = pact.WithHttpInteractions();
    }

    [Test]
    public async Task VerifyWithValidCredentials()
    {
        this.pact
                .UponReceiving("given a valid credentials")
                    .WithRequest(HttpMethod.Post, "/connect/token")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        client_id = "CSClient1",
                        client_secret = "test123@Puiu"
                    })
                .WillRespond()
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithStatus(HttpStatusCode.OK);

        await this.pact.VerifyAsync(async ctx =>
        {
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(
                   (collection) =>
                   {
                       collection.AddTransient((x) => new TokenRequest.Validator());
                       collection.AddTransient((x) => _tokenManager!);
                   }
               );
            });

            using var client = factory.CreateClient();

            // Act
            var requestContent = new { clientid = "CSClient1", clientsecret = "test123@Puiu" };
            var response = await client.PostAsJsonAsync("/connect/token", requestContent);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNull();
            content.Should().Contain("input123token");
            content.Should().Contain("bearer");
        });
    }
}
