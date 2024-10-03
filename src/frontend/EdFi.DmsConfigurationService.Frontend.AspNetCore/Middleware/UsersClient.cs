// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Middleware;

public class UsersClient : IUsersClient
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IHttpClientFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersClient"/> class.
    /// </summary>
    /// <param name="factory">HTTP client factory</param>
    public UsersClient(IHttpClientFactory factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Get an user by username
    /// </summary>
    /// <param name="Username">Username</param>
    /// <returns>UserDto</returns>
    public async Task<UserDto?> GetUserAsync(string Username)
    {
        using HttpClient client = factory.CreateClient("Users");
        return await client.GetFromJsonAsync<UserDto?>($"/users/{Username}", _options);
        //UserDto user = await client.GetFromJsonAsync<UserDto>($"/users/{Username}", _options);
        //return user;
    }
}
