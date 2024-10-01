// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Keycloak.Net;
using Keycloak.Net.Models.Clients;
using Keycloak.Net.Models.Roles;
using Keycloak.Net.Models.Users;

namespace EdFi.DmsConfigurationService.Backend.Keycloak;

public class UserRepository(KeycloakContext keycloakContext) : IUserRepository
{
    private readonly KeycloakClient _keycloakClient = new(keycloakContext.Url, keycloakContext.ClientSecret,
        new KeycloakOptions(adminClientId: keycloakContext.ClientId));
    private readonly string _realm = keycloakContext.Realm!;

    public async Task<bool> CreateClientAsync(string clientId, string clientSecret, string displayName)
    {
        try
        {
            var client = new Client
            {
                ClientId = clientId,
                Enabled = true,
                Secret = clientSecret,
                Name = displayName,
                ServiceAccountsEnabled = true
            };
            var createdClient = await _keycloakClient.CreateClientAndRetrieveClientIdAsync(_realm, client);
            return string.IsNullOrEmpty(createdClient) ? throw new Exception($"Error while creating the client: {clientId}") : true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> CreateUserAsync(string userName, string email, string password)
    {
        try
        {
            var users = await GetAllUsersAsync();
            var userRole = new Role();
            var newUser = new User
            {
                UserName = userName,
                Email = email,
                Enabled = true,
                EmailVerified = true
            };
            var roles = await _keycloakClient.GetRolesAsync(_realm);
            if (users != null && users.Any())
            {
                userRole = GetRole("configuration-user");
            }
            else
            {
                userRole = GetRole("configuration-admin");
            }

            var userCreatedResponse = await _keycloakClient.CreateUserAsync(_realm, newUser);
            if (userCreatedResponse == true)
            {
                var createdUser = await GetAllUserByUserNameAsync(userName);
                if (createdUser != null)
                {
                    if (userRole != null)
                    {
                        await _keycloakClient.AddRealmRoleMappingsToUserAsync(_realm, createdUser.Id, [userRole]);
                    }
                    await _keycloakClient.SetUserPasswordAsync(_realm, createdUser.Id, password);
                    return true;
                }
            }
            Role? GetRole(string roleName) => roles.FirstOrDefault(x => x.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return false;
    }

    private async Task<User?> GetAllUserByUserNameAsync(string userName)
    {
        var users = await _keycloakClient.GetUsersAsync(_realm, username: userName);
        if (users != null && users.Any())
        {
            return users.FirstOrDefault();
        }
        return null;
    }

    private async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _keycloakClient.GetUsersAsync(_realm);
    }

    private async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await _keycloakClient.GetClientsAsync(_realm);
    }
}
