// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FluentValidation;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Model;

public class TokenRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public class Validator : AbstractValidator<TokenRequest>
    {
        public Validator()
        {
            RuleFor(m => m.Username).NotEmpty();
            RuleFor(m => m.Password).NotEmpty();
        }
    }
}

public class TokenClientRequest
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    public class Validator : AbstractValidator<TokenClientRequest>
    {
        public Validator()
        {
            RuleFor(m => m.ClientId).NotEmpty();
            RuleFor(m => m.ClientSecret).NotEmpty();
        }
    }
}

