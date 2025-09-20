// <copyright file="UserActor{Logins}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Actors;

using System.Collections.Generic;

using Hexalith.IdentityStores.Models;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

/// <summary>
/// Actor responsible for managing user identity operations in a Dapr-based identity store.
/// This actor handles CRUD operations for user identities and maintains associated indexes.
/// </summary>
public partial class UserActor
{
    /// <summary>
    /// Adds a third-party login provider to the user's account.
    /// </summary>
    /// <param name="login">Login information containing provider and key.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    public async Task AddLoginAsync(CustomUserLoginInfo login)
    {
        ArgumentNullException.ThrowIfNull(login);
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None).ConfigureAwait(false);
        if (_state is null)
        {
            throw new InvalidOperationException($"Add login failed : User '{userId}' not found.");
        }

        _state.Logins = _state
            .Logins
            .Where(p => p.LoginProvider != login.LoginProvider || p.ProviderKey != login.ProviderKey)
            .Append(login);

        await StateManager.SetStateAsync(IdentityStoresConstants.UserStateName, _state, CancellationToken.None).ConfigureAwait(false);
        await StateManager.SaveStateAsync(CancellationToken.None).ConfigureAwait(false);

        await _loginIndexService.AddAsync(login.LoginProvider, login.ProviderKey, userId).ConfigureAwait(false);
    }

    /// <summary>
    /// Finds a specific login provider entry for the user.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="providerKey">Unique key from the provider.</param>
    /// <returns>Login information if found, null otherwise.</returns>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    public async Task<CustomUserLogin?> FindLoginAsync(string loginProvider, string providerKey)
    {
        _state = await GetStateAsync(CancellationToken.None).ConfigureAwait(false);
        return _state is null
            ? throw new InvalidOperationException($"Find login failed : User '{Id.ToUnescapeString()}' not found.")
            : _state.Logins
                .Where(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey)
                .Select(p => new CustomUserLogin(p, _state.User.Id))
                .FirstOrDefault();
    }

    /// <summary>
    /// Gets all external login providers associated with the user.
    /// </summary>
    /// <returns>Collection of login provider information.</returns>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    public async Task<IEnumerable<CustomUserLoginInfo>> GetLoginsAsync()
    {
        _state = await GetStateAsync(CancellationToken.None).ConfigureAwait(false);
        return (_state
                ?? throw new InvalidOperationException($"Get logins failed : User '{Id.ToUnescapeString()}' not found."))
            .Logins;
    }

    /// <summary>
    /// Removes a specific login provider from the user's account.
    /// </summary>
    /// <param name="loginProvider">Name of the login provider.</param>
    /// <param name="providerKey">Unique key from the provider.</param>
    /// <exception cref="InvalidOperationException">When user not found.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveLoginAsync(string loginProvider, string providerKey)
    {
        string userId = Id.ToUnescapeString();
        _state = await GetStateAsync(CancellationToken.None).ConfigureAwait(false);
        if (_state is null)
        {
            throw new InvalidOperationException($"Remove login Failed : User '{userId}' not found.");
        }

        _state.Logins = _state.Logins.Where(p => p.ProviderKey != providerKey || p.LoginProvider != loginProvider);
        await _loginIndexService.RemoveAsync(loginProvider, providerKey).ConfigureAwait(false);
        await StateManager.SetStateAsync(IdentityStoresConstants.UserStateName, _state, CancellationToken.None).ConfigureAwait(false);
        await StateManager.SaveStateAsync(CancellationToken.None).ConfigureAwait(false);
    }
}