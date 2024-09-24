// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Claims;
using System.Text.Json.Nodes;
using EdFi.DmsConfigurationService.Frontend.AspNetCore.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
    {
        IConfiguration config = webApplicationBuilder.Configuration;

        var settings = config.GetSection("IdentitySettings");
        var identitySettings = new IdentitySettings();
        settings.Bind(identitySettings);

        webApplicationBuilder.Services.Configure<IdentitySettings>(settings);

        webApplicationBuilder.Services.AddHttpClient();

        var authority = identitySettings.Authority;
        var metadataAddress = $"{authority}/.well-known/openid-configuration";

        webApplicationBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.MetadataAddress = metadataAddress;
                options.Authority = authority;
                options.Audience = "account";
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        List<AuthenticationToken> tokens = context.Properties!.GetTokens().ToList();
                        ClaimsIdentity claimsIdentity = (ClaimsIdentity)context.Principal!.Identity!;

                        var realm_access = claimsIdentity.FindFirst((claim) => claim.Type == "realm_access")?.Value;

                        if (realm_access != null)
                        {
                            JsonNode? node = JsonNode.Parse(realm_access);
                            var roleAccess = node!["roles"] as JsonArray;
                            foreach (var role in roleAccess!)
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role!.ToString()));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        webApplicationBuilder.Services.AddAuthorization(options => options.AddPolicy(SecurityConstants.AdminPolicy,
            policy => policy.RequireClaim(ClaimTypes.Role, SecurityConstants.AdminRole)));
    }
}
