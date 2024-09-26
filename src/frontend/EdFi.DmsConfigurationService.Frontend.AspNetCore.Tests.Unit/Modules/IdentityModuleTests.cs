// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Net.Http.Json;
using EdFi.DmsConfigurationService.Backend;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Tests.Unit.Modules;

[TestFixture]
public class RegisterEndpointTests
{
    private IUserRepository? _userRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = A.Fake<IUserRepository>();
        A.CallTo(() => _userRepository.CreateUserAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(Task.FromResult(true));
    }

    [Test]
    public async Task Given_valid_user_details()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                    collection.AddTransient((x) => _userRepository!);
                }
            );
        });
        using var client = factory.CreateClient();

        // Act
        var requestContent = new { username = "CSUser2", password = "test123@Puiu", emailid = "CSUser2@dms.com" };
        var response = await client.PostAsJsonAsync("/connect/register", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task Given_invalid_user_emailid()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                    collection.AddTransient((x) => _userRepository!);
                }
            );
        });
        using var client = factory.CreateClient();

        // Act
        var requestContent = new { username = "CSUser2", password = "test123@Puiu", emailid = "CSUser2" };
        var response = await client.PostAsJsonAsync("/connect/register", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("Please provide valid email id");
    }

    [Test]
    public async Task Given_empty_user_details()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                    collection.AddTransient((x) => _userRepository!);
                }
            );
        });
        using var client = factory.CreateClient();

        // Act
        var requestContent = new { username = "", password = "", emailid = "" };
        var response = await client.PostAsJsonAsync("/connect/register", requestContent);
        var content = await response.Content.ReadAsStringAsync();
        content = System.Text.RegularExpressions.Regex.Unescape(content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("'Username' must not be empty.");
        content.Should().Contain("'Password' must not be empty.");
        content.Should().Contain("'Email Id' must not be empty.");
    }

    [Test]
    [TestCase("sM@1l")]
    [TestCase("VeryVeryVeryLongPasswordM@1l")]
    [TestCase("noupperc@s3")]
    [TestCase("NOLOWERC@S3")]
    [TestCase("NoSpecial0908")]
    [TestCase("NoNumberP@ssWord")]
    public async Task Given_invalid_user_password(string password)
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                    collection.AddTransient((x) => _userRepository!);
                }
            );
        });
        using var client = factory.CreateClient();

        // Act
        var requestContent = new { username = "CSUser2", password, emailid = "CSUser2@cs.com" };
        var response = await client.PostAsJsonAsync("/connect/register", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character, and must be 8 to 12 characters long.");
    }

    [Test]
    public async Task When_error_from_backend()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            _userRepository = A.Fake<IUserRepository>();
            A.CallTo(() => _userRepository.CreateUserAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
            .Throws(new Exception("Error from Keycloak"));

            builder.UseEnvironment("Test");
            builder.ConfigureServices(
                (collection) =>
                {
                    collection.AddTransient((x) => new RegisterRequest.Validator());
                    collection.AddTransient((x) => _userRepository!);
                }
            );
        });
        using var client = factory.CreateClient();

        // Act
        var requestContent = new { username = "CSUser2", password = "test123@Puiu", emailid = "CSUser2@cm.com" };
        var response = await client.PostAsJsonAsync("/connect/register", requestContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().Contain("Error from Keycloak");
    }
}

[TestFixture]
public class TokenEndpointTests
{
    private ITokenManager? _tokenManager;

    [SetUp]
    public void Setup()
    {
        _tokenManager = A.Fake<ITokenManager>();
        A.CallTo(() => _tokenManager.GetUserAccessTokenAsync(A<IEnumerable<KeyValuePair<string, string>>>.Ignored)).Returns(Task.FromResult(""));
    }
}
