// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;
using Match = PactNet.Matchers.Match;


namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

public class ConsumerPactAuthorizationFixture
{

    private readonly IPactBuilderV2 pact;

    public ConsumerPactAuthorizationFixture(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new[]
            {
                    new XunitOutput(output)
                },
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        IPactV2 pact = Pact.V2("DMS Configuration API Consumer", "DMS API", config);
        this.pact = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task GetAllEvents_WithNoAuthorizationToken_ShouldFail()
    {
        this.pact
            .UponReceiving("a request to retrieve all events with no authorization")
                .Given("there is an existing user'")
                .WithRequest(HttpMethod.Get, "/admin")
                .WithHeader("Accept", "application/json")
            .WillRespond()
                .WithStatus(HttpStatusCode.Unauthorized);

        await this.pact.VerifyAsync(ctx =>
        {
            Thread.Sleep(5);
            return Task.CompletedTask;
        });
        //TODO
        /* await this.pact.VerifyAsync(async ctx =>
        {
            var client = new EventsApiClient(ctx.MockServerUri);

            await client.Invoking(c => c.GetAllEvents()).Should().ThrowAsync<Exception>();
        }); */
    }

}
