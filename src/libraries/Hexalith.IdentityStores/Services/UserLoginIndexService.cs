// <copyright file="UserLoginIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Services;

using System;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.IdentityStores.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity logins in a collection.
/// This service handles the mapping between user IDs and their external login providers using Dapr actors.
/// It provides functionality to add, find, and remove login provider mappings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserLoginIndexService"/> class.
/// This constructor is used in production with actual Dapr actor implementation.
/// </remarks>
/// <param name="factory">The Dapr actor host providing actor management capabilities.</param>
public class UserLoginIndexService(IActorProxyFactory factory) : IUserLoginIndexService
{
    /// <summary>
    /// Factory function to create key-value actors for login indexing.
    /// </summary>
    private readonly Func<string, string, IKeyValueActor> _keyValueActor = factory.CreateUserLoginIndexProxy;

    /// <inheritdoc/>
    public async Task AddAsync(string loginProvider, string providerKey, string userId) =>
        await _keyValueActor(loginProvider, providerKey).SetAsync(userId).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<string?> FindUserIdAsync(string loginProvider, string providerKey) =>
        await _keyValueActor(loginProvider, providerKey).GetAsync().ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task RemoveAsync(string loginProvider, string providerKey)
        => await _keyValueActor(loginProvider, providerKey).RemoveAsync().ConfigureAwait(false);
}