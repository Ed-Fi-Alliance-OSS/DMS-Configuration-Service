// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Tests.Unit.Modules;

[TestFixture]
public class RegisterActionEndpointTests
{

    [SetUp]
    public void Setup() { }

    [Test]
    public async Task Given_valid_path_and_parameters()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                }
            );
        });
        using var client = factory.CreateClient();

        var testActionResponse = new ActionResponse[] {
            new ActionResponse {Id = 1, Name = "Create", Uri = "uri://ed-fi.org/odsapi/actions/create"},
            new ActionResponse {Id = 2, Name = "Read", Uri = "uri://ed-fi.org/odsapi/actions/read"},
            new ActionResponse {Id = 3, Name = "Update", Uri = "uri://ed-fi.org/odsapi/actions/update"},
            new ActionResponse {Id = 4, Name = "Delete", Uri = "uri://ed-fi.org/odsapi/actions/delete"}
        };

        // Act
        var response = await client.GetAsync("/v2/actions");
        var content = await response.Content.ReadAsStringAsync();
        var deserialized = JsonConvert.DeserializeObject<ActionResponse[]>(content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        deserialized.Should().BeEquivalentTo(testActionResponse);
    }
}
