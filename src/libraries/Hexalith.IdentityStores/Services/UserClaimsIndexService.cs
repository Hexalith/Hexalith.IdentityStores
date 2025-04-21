// <copyright file="UserClaimsIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Services;

using Dapr.Actors.Client;

using Hexalith.IdentityStores.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Actors;

/// <summary>
/// Service for managing user identity claims indexing using Dapr actors.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserClaimsIndexService"/> class.
/// </remarks>
/// <param name="actorProxyFactory">The actor proxy factory instance.</param>
public class UserClaimsIndexService(
    IActorProxyFactory actorProxyFactory) : IUserClaimsIndexService
{
    private readonly Func<string, string?, IKeyHashActor> _keyValueActor = actorProxyFactory.CreateClaimUsersIndexProxy;

    /// <inheritdoc/>
    public async Task AddAsync(string claimType, string? claimValue, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(userId);

        _ = await _keyValueActor(claimType, claimValue).AddAsync(userId).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> FindUserIdsAsync(string claimType, string? claimValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);

        return await _keyValueActor(claimType, claimValue).AllAsync(0, 0).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string claimType, string? claimValue, string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(userId);

        await _keyValueActor(claimType, claimValue).RemoveAsync(userId).ConfigureAwait(false);
    }
}