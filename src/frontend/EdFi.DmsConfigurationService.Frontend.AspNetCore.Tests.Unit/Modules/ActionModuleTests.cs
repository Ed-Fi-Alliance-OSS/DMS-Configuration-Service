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
using System.Text.Json;
using NUnit.Framework;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Tests.Unit.Modules;

[TestFixture]
public class RegisterActionEndpointTests
{
    [TestFixture]
    public class Given_A_valid_action_request
    {
        private AdminAction[] _mockActionResponse;
        private HttpResponseMessage? _response;
        private AdminAction[]? _content;

        [SetUp]
        public async Task Setup()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });
            var client = factory.CreateClient();

            _mockActionResponse = [
                new AdminAction {Id = 1, Name = "Create", Uri = "uri://ed-fi.org/api/actions/create"},
                new AdminAction {Id = 2, Name = "Read", Uri = "uri://ed-fi.org/api/actions/read"},
                new AdminAction {Id = 3, Name = "Update", Uri = "uri://ed-fi.org/api/actions/update"},
                new AdminAction {Id = 4, Name = "Delete", Uri = "uri://ed-fi.org/api/actions/delete"}
            ];

            // Act
            _response = await client.GetAsync("/v2/actions");
            var responseString = await _response.Content.ReadAsStringAsync();
            _content = JsonSerializer.Deserialize<AdminAction[]>(responseString);
        }

        [Test]
        public void When_response_is_provide()
        {
            // Assert
            _response!.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void When_response_payload_is_provided()
        {
            // Assert
            _content.Should().BeEquivalentTo(_mockActionResponse);
        }

        [TearDown]
        public void TearDown()
        {
            _response!.Dispose();
        }
    }

};

