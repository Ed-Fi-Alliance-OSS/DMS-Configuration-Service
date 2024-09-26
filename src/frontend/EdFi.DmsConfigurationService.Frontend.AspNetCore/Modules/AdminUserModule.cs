// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;

public class AdminUserModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/admin", GetAdminDetails).RequireAuthorization(SecurityConstants.AdminPolicy);
        endpoints.MapGet("/user", GetUserDetails).RequireAuthorization();
    }

    public IResult GetAdminDetails(HttpContext httpContext)
    {
        var currentUser = httpContext.User;
        return Results.Ok($"Admin user name: {currentUser.Claims.First(x => x.Type.Equals("preferred_username")).Value}");
    }

    public IResult GetUserDetails(HttpContext httpContext)
    {
        var currentUser = httpContext.User;
        return Results.Ok($"User name: {currentUser.Claims.First(x => x.Type.Equals("preferred_username")).Value}");
    }
}
