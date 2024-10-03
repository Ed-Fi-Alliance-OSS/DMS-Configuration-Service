// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.Middleware
{
    /// <summary>
    /// Username DTO
    /// </summary>
    /// <param name="Username">Username</param>
    /// <param name="Password">Password</param>
    /// <param name="Email">Email</param>
    public record UserDto(string Username, string Password, string Email);
}
