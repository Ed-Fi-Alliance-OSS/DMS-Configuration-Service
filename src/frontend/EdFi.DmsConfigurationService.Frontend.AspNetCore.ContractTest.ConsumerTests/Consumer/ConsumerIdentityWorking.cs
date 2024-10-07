// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using PactNet;
using EdFi.DmsConfigurationService.Backend;
using System.Net.Http.Json;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

public class ConsumerIdentityWorkingTest
{
    private readonly IPactBuilderV4 pact;

    public ConsumerIdentityWorkingTest()
    {
        pact = Pact.V4("DMS API Consumer", "DMS Configuration Service API").WithHttpInteractions();
    }

    [Test]
    public async Task VerifyWithValidCredentials()
    {
        pact.UponReceiving("given a valid credentials")
            .WithRequest(HttpMethod.Post, "/connect/token-test")
            .WithJsonBody(new
            {
                clientid = "CSClient1",
                clientsecret = "test123@Puiu"
            })
            .WithHeader("Content-Type", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                access_token = "input123token",
                expires_in = 900,
                token_type = "bearer"
            });

        await pact.VerifyAsync(async ctx =>
        {
            var client = new HttpClient();

            // Act
            var requestBody = new { clientid = "CSClient1", clientsecret = "test123@Puiu" };
            var response = await client.PostAsJsonAsync($"{ctx.MockServerUri}connect/token-test", requestBody);

        });
    }
}
