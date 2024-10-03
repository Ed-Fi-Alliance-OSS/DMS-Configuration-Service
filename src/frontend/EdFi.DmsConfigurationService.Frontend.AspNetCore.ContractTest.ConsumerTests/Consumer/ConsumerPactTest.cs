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

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

[TestFixture]
public class ConsumerPactTest
{
    private readonly IPactBuilderV3 pact;
    //private RequestDelegate _next;
    //private ILogger<RequestLoggingMiddleware> _logger;

    private readonly Mock<IHttpClientFactory> mockFactory;

    public ConsumerPactTest()
    {
        this.mockFactory = new Mock<IHttpClientFactory>();

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
    public async Task ItHandles404ErrorForNonExistingResource()
    {
        this.pact
                .UponReceiving("A request for a non-existing resource")
                    .WithRequest(HttpMethod.Get, "/users/123")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

        await this.pact.VerifyAsync(async ctx =>
        {
            this.mockFactory
                .Setup(f => f.CreateClient("Users"))
                .Returns(() => new HttpClient
                {
                    BaseAddress = ctx.MockServerUri
                });

            var client = new UsersClient(mockFactory.Object);

            Func<Task> action = () => client.GetUserAsync("123");

            var response = await action.Should().ThrowAsync<HttpRequestException>();
            response.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
        });
    }
}
