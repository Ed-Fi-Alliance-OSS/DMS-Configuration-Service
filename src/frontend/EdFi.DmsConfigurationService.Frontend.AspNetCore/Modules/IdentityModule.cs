// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.DmsConfigurationService.Frontend.AspNetCore.Configuration;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;

public class IdentityModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/oauth/token", GenerateToken);
    }

    public async Task<IResult> GenerateToken(TokenRequest model, IOptions<IdentitySettings> identitySettings, IHttpClientFactory httpClientFactory)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();

            var tokenEndpoint = "http://localhost:8045/realms/dms/protocol/openid-connect/token";
            var settings = identitySettings.Value;
            var requestContent = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", settings.ClientId!),
                new KeyValuePair<string, string>("client_secret", settings.ClientSecret!),
                new KeyValuePair<string, string>("username", model.Username!),
                new KeyValuePair<string, string>("password", model.Password!)
            ]);

            var response = await httpClient.PostAsync(tokenEndpoint, requestContent);
            if (response.IsSuccessStatusCode)
            {

                var accessToken = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<TokenResponse>(accessToken);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token!.AccessToken);
                return Results.Ok(token);
            }
            else
            {
                return Results.Unauthorized();
            }
        }
        catch (Exception)
        {
            return Results.StatusCode(500);
        }
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
}

