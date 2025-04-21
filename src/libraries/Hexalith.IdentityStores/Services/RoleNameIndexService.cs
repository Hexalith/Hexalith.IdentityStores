// <copyright file="RoleNameIndexService.cs" company="ITANEO">
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
/// Service for managing role identity names in a collection.
/// This service handles the mapping between role IDs and their role names using Dapr actors.
/// It provides functionality to add, find, and remove role name-to-roleId mappings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleNameIndexService"/> class.
/// This constructor is used in production with actual Dapr actor implementation.
/// </remarks>
/// <param name="factory">The Dapr actor host providing actor management capabilities.</param>
public class RoleNameIndexService(IActorProxyFactory factory) : IRoleNameIndexService
{
    /// <summary>
    /// Factory function to create key-value actors for role name indexing.
    /// </summary>
    private readonly Func<string, IKeyValueActor> _keyValueActor = factory.CreateRoleNameIndexProxy;

    /// <summary>
    /// Associates a role ID with a role name in the actor state store.
    /// </summary>
    /// <param name="name">The role name to associate with the role.</param>
    /// <param name="roleId">The role's unique identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddAsync(string name, string roleId)
        => await _keyValueActor(name).SetAsync(roleId).ConfigureAwait(false);

    /// <summary>
    /// Retrieves a role ID associated with the given role name.
    /// </summary>
    /// <param name="name">The role name to look up.</param>
    /// <returns>The associated role ID if found; otherwise, null.</returns>
    public async Task<string?> FindRoleIdAsync(string name)
        => await _keyValueActor(name).GetAsync().ConfigureAwait(false);

    /// <summary>
    /// Removes the association between a role ID and a role name.
    /// </summary>
    /// <param name="name">The role name to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RemoveAsync(string name)
        => await _keyValueActor(name).RemoveAsync().ConfigureAwait(false);
}