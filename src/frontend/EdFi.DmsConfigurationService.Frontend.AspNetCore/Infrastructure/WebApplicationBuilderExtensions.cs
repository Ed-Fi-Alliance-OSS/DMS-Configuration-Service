// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
    {
        // For FluentValidation
        var executingAssembly = Assembly.GetExecutingAssembly();

        webApplicationBuilder.Services
         .AddValidatorsFromAssembly(executingAssembly)
         .AddFluentValidationAutoValidation();
        ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression)
                    => memberInfo?
                        .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
    }
}
