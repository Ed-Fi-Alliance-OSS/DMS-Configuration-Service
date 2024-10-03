// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Modules;

public class HealthModule : IEndpointModule
{
    private readonly IHttpClientFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Health"/> class.
    /// </summary>
    /// <param name="factory">HTTP client factory</param>
    public HealthModule(IHttpClientFactory factory)
    {
        this.factory = factory;
    }
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", () => Results.Text(DateTime.Now.ToString()));
    }
}
