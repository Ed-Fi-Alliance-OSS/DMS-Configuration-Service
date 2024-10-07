// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EdFi.DmsConfigurationService.Backend;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Microsoft.Extensions.Logging;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.ContractTest;

public class ConsumerIdentityTest
{
    private IPactBuilderV4? pact;
    private ITokenManager? _tokenManager;

    [SetUp]
    public void Setup()
    {
        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new List<IOutput>
            {
                new ConsoleOutput(),
                new FileOutput("../../../logs/pact-log.txt")  // Logs de Pact
            },
            DefaultJsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            },
            LogLevel = PactLogLevel.Debug
        };

        pact = Pact.V4("DMS API Consumer", "DMS Configuration Service API", config).WithHttpInteractions();

        _tokenManager = A.Fake<ITokenManager>();
        var token = """
            {
                "access_token":"input123token",
                "expires_in":900,
                "token_type":"bearer"
            }
            """;
        A.CallTo(
                () => _tokenManager.GetAccessTokenAsync(A<IEnumerable<KeyValuePair<string, string>>>.Ignored)
            )
            .Returns(Task.FromResult(token));
    }

    [Test]
    public async Task VerifyWithValidCredentials()
    {
        // Sin uso de Pact.Matchers
        pact?.UponReceiving("given a valid credentials")
            .WithRequest(HttpMethod.Post, "/connect/token")
            .WithHeader("Content-Type", "application/json") // Asegurarse de que el encabezado coincida
            .WithJsonBody(new { clientid = "CSClient1", clientsecret = "test123@Puiu" })
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(new // Cuerpo de la respuesta
            {
                access_token = "input123token",
                expires_in = 900,
                token_type = "bearer"
            });

        await pact!.VerifyAsync(async ctx =>
        {
            await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddTransient((x) => new TokenRequest.Validator());
                    services.AddTransient((x) => _tokenManager!);
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
            });

            using var client = factory.CreateClient();

            // Act
            var requestContent = new { clientid = "CSClient1", clientsecret = "test123@Puiu" };
            var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token")
            {
                Content = JsonContent.Create(requestContent)
            };
            request.Content.Headers.Clear();
            request.Content.Headers.Add("Content-Type", "application/json");

            Console.WriteLine("Request JSON:");
            Console.WriteLine(await request.Content.ReadAsStringAsync());

            // Enviar la solicitud
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response Status: " + response.StatusCode);
            Console.WriteLine("Response Body: " + content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        });
    }
}

public class FileOutput : IOutput
{
    private readonly string _filePath;

    public FileOutput(string filePath)
    {
        _filePath = filePath;
    }

    public void Write(string message)
    {
        File.AppendAllText(_filePath, message);
    }

    public void WriteLine(string message)
    {
        File.AppendAllText(_filePath, message + "\n");
    }
}
