// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Options;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Configuration;

public class IdentitySettings
{
    public required string Authority { get; set; }
    public required string IdentityServer { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Realm { get; set; }
    public string? UserProfileUri { get; set; }
    public string? AuthenticationScheme { get; set; }
    public string? LoginProvider { get; set; }
    public string? ResponseType { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public bool GetClaimsFromUserInfoEndpoint { get; set; }
    public bool SaveTokens { get; set; }
    public List<string>? Scopes { get; set; }
}

public class IdentitySettingsValidator : IValidateOptions<IdentitySettings>
{
    public ValidateOptionsResult Validate(string? name, IdentitySettings options)
    {
        if (string.IsNullOrWhiteSpace(options.Authority))
        {
            return ValidateOptionsResult.Fail("Missing required IdentitySettings value: Authority");
        }

        if (string.IsNullOrWhiteSpace(options.IdentityServer))
        {
            return ValidateOptionsResult.Fail("Missing required IdentitySettings value: IdentityServer");
        }

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            return ValidateOptionsResult.Fail("Missing required IdentitySettings value: ClientId");
        }
        if (string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            return ValidateOptionsResult.Fail("Missing required IdentitySettings value: ClientSecret");
        }
        if (string.IsNullOrWhiteSpace(options.Realm))
        {
            return ValidateOptionsResult.Fail("Missing required IdentitySettings value: Realm");
        }
        return ValidateOptionsResult.Success;
    }
}
