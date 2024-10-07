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
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging.Console;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

public class ConsumerIdentityTest
{
    private readonly IPactBuilderV4 pact;
    //private RequestDelegate _next;
    //private ILogger<RequestLoggingMiddleware> _logger;

    private Mock<IHttpClientFactory> _mockFactory;

    private ITokenManager? _tokenManager;

    /* [SetUp]
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
    } */

    public ConsumerIdentityTest()
    {
        this._mockFactory = new Mock<IHttpClientFactory>();

        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new List<IOutput> { new ConsoleOutput() },
            /* Outputters = new List<IOutput>
            {
                new NUnitOutput()
            }, */
            DefaultJsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            },
            LogLevel = PactLogLevel.Debug
        };

        pact = Pact.V4("DMS API Consumer", "DMS Configuration Service API", config).WithHttpInteractions();
        //this.pact = pact.WithHttpInteractions();

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

    [Test]
    public async Task VerifyWithValidCredentials()
    {
        this.pact
                .UponReceiving("given a valid credentials")
                    .WithRequest(HttpMethod.Post, "/connect/token")
                    //.WithHeader("Content-Type", "application/json;")
                    .WithJsonBody(new
                    {
                        //client_id = "CSClient1",
                        //client_secret = "test123@Puiu"
                        clientid = "CSClient1",
                        clientsecret = "test123@Puiu"
                        //granttype = "password",
                        //username = "123",
                        //password = "12345"
                    })
                .WillRespond()
                    //.WithHeader("Content-Type", "application/json;")
                    .WithStatus(HttpStatusCode.OK);

        await this.pact.VerifyAsync(async ctx =>
        {
            /*_mockFactory = new Mock<IHttpClientFactory>()*/
            var mockServerUri = ctx.MockServerUri;

            var mockHttpClient = new HttpClient
            {
                BaseAddress = new Uri(mockServerUri.ToString()) // Set the mock server's base address
            };

            //_mockFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IHttpClientFactory>(_mockFactory.Object);
                    services.AddTransient((x) => new TokenRequest.Validator());
                    services.AddTransient((x) => _tokenManager!);
                });
            });

            using var client = factory.CreateClient();
            client.BaseAddress = ctx.MockServerUri;

            // Act
            var requestContent = new { clientid = "CSClient1", clientsecret = "test123@Puiu" }; // Verification mismatches
            //var requestContent = new { clientid = "CSClient1", clientsecret = "test123@Puiu", granttype = "password" }; // Verification mismatches
            //var requestContent = new { clientid = "CSClient1", clientsecret = "test123@Puiu", grant_type = "password" };
            //var requestContent = new { client_id = "CSClient1", client_secret = "test123@Puiu", grant_type = "password" }; //BAD REQUEST
            //var requestContent = new { client_id = "CSClient1", client_secret = "test123@Puiu", grant_type = "password", username = "123", password = "12345" }; ////BAD REQUEST
            var response = await client.PostAsJsonAsync("/connect/token", requestContent);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            //content.Should().NotBeNull();
            //content.Should().Contain("input123token");
            //content.Should().Contain("bearer");
        });
    }
}
