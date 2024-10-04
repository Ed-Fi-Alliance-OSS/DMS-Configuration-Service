// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;
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

            // TODO(): Update response to use a Data Transfer Object.
            var response = new ActionResponse[] {
                    new ActionResponse {Id = 1, Name = "Create", Uri = "uri://ed-fi.org/odsapi/actions/create"},
                    new ActionResponse {Id = 2, Name = "Read", Uri = "uri://ed-fi.org/odsapi/actions/read"},
                    new ActionResponse {Id = 3, Name = "Update", Uri = "uri://ed-fi.org/odsapi/actions/update"},
                    new ActionResponse {Id = 4, Name = "Delete", Uri = "uri://ed-fi.org/odsapi/actions/delete"},
                };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            throw new IdentityException($"Get actions failed with: {ex.Message}");
        }
    }
}
