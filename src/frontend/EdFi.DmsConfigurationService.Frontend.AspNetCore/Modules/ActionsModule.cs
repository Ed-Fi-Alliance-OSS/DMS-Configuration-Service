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

public class ActionsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v2/actions", GetUserActions);
    }

    public IResult GetUserActions(HttpContext httpContext)
    {
        try
        {
            // TODO(): Ensure the request contains a non-expired token with correct issuer and audience. Otherwise 401.

            // TODO(): Ensure claims and role are valid for an admin client account with the role Configuration Service Admin. Otherwise 403.

            // TODO(): Return a JSON list of valid actions  according to Admin API 2's spec.
            return Results.Ok("results");
        }
        catch (Exception ex)
        {
            throw new IdentityException($"Get actions failed with: {ex.Message}");
        }
    }
}
