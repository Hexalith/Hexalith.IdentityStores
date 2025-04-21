// <copyright file="DaprActorRoleStore{Roles}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Stores;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapr.Actors.Client;

using Hexalith.IdentityStores.Actors;
using Hexalith.IdentityStores.Helpers;
using Hexalith.IdentityStores.Models;
using Hexalith.IdentityStores.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
/// </summary>
/// <param name="roleCollection">The role identity collection service.</param>
/// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe identity errors.</param>
public partial class DaprActorRoleStore(IRoleCollectionService roleCollection, IdentityErrorDescriber describer) : RoleStoreBase<CustomRole, string, CustomUserRole, CustomRoleClaim>(describer)
{
    private readonly IRoleCollectionService _roleCollection = roleCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaprActorRoleStore"/> class.
    /// </summary>
    /// <param name="roleCollection">The role identity collection service.</param>
    public DaprActorRoleStore(
        IRoleCollectionService roleCollection)
        : this(roleCollection, new IdentityErrorDescriber())
    {
        ArgumentNullException.ThrowIfNull(roleCollection);
        _roleCollection = roleCollection;
    }

    /// <inheritdoc/>
    public override IQueryable<CustomRole> Roles
    {
        get
        {
            ThrowIfDisposed();
            return GetRolesAsync().GetAwaiter().GetResult().AsQueryable();
        }
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        bool created = await actor.CreateAsync(role).ConfigureAwait(false);
        return created
            ? IdentityResult.Success
            : IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.Name ?? role.Id));
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.DeleteAsync().ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public override async Task<CustomRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(id);
        return await actor.FindAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CustomRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedName);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        IKeyValueActor actor = ActorProxy.DefaultProxyFactory.CreateRoleNameIndexProxy(normalizedName);
        string? roleId = await actor.GetAsync().ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(roleId) ? null : await FindByIdAsync(roleId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (await FindByIdAsync(role.Id, cancellationToken).ConfigureAwait(false) == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Role '{role.Id}' not found.",
            });
        }

        IRoleActor actor = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(role.Id);
        await actor.UpdateAsync(role).ConfigureAwait(false);
        return IdentityResult.Success;
    }

    /// <summary>
    /// Gets the list of roles asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of roles.</returns>
    private async Task<List<CustomRole>> GetRolesAsync()
    {
        ThrowIfDisposed();

        IEnumerable<string> roleIds = await _roleCollection.AllAsync().ConfigureAwait(false);
        List<Task<CustomRole?>> tasks = [];
        foreach (string roleId in roleIds)
        {
            IRoleActor roleProxy = ActorProxy.DefaultProxyFactory.CreateRoleIdentityActor(roleId);
            tasks.Add(roleProxy.FindAsync());
        }

        return [.. (await Task.WhenAll(tasks).ConfigureAwait(false))
            .Where(p => p != null)
            .OfType<CustomRole>()];
    }
}