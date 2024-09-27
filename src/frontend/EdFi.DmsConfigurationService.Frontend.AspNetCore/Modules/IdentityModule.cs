// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.DmsConfigurationService.Backend;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;

public class IdentityModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/connect/register", RegisterUser);
        endpoints.MapPost("/connect/token", GetAccessToken);
    }

    public async Task<IResult> RegisterUser(RegisterRequest.Validator validator, RegisterRequest model, IUserRepository userRepository)
    {
        try
        {
            await validator.GuardAsync(model);
            await userRepository.CreateUserAsync(model.Username!, model.EmailId!, model.Password!);
            return Results.Created();
        }
        catch (Exception ex)
        {
            throw new IdentityException($"User registration failed with:{ex.Message}");
        }
    }

    public async Task<IResult> GetAccessToken(TokenRequest.Validator validator, TokenRequest model, ITokenManager tokenManager)
    {
        try
        {
            await validator.GuardAsync(model);
            var response = await tokenManager.GetUserAccessTokenAsync([
                new KeyValuePair<string, string>("username", model.Username!),
                new KeyValuePair<string, string>("password", model.Password!)
                ]);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response);
            return Results.Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            throw new IdentityException($"User token generation failed with:{ex.Message}");
        }
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}

