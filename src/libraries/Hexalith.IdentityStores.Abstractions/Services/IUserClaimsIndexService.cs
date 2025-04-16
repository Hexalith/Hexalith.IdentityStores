﻿// <copyright file="IUserClaimsIndexService.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Services;

/// <summary>
/// Service interface for managing user identity claims indexing.
/// </summary>
/// <remarks>
/// This service provides functionality to maintain and query indexes of user claims,
/// enabling efficient lookups of users by their claim values.
/// </remarks>
public interface IUserClaimsIndexService
{
    /// <summary>
    /// Adds a claim value to the user's index.
    /// </summary>
    /// <param name="claimType">The type of the claim being indexed.</param>
    /// <param name="claimValue">The value of the claim being indexed.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string claimType, string? claimValue, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds all user IDs associated with a specific claim type and value.
    /// </summary>
    /// <param name="claimType">The type of claim to search for.</param>
    /// <param name="claimValue">The value of the claim to search for.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A collection of user IDs that have the specified claim type and value.</returns>
    /// <remarks>
    /// This method provides a way to search for users by specifying the claim type and value separately,
    /// rather than passing a Claim object.
    /// </remarks>
    Task<IEnumerable<string>> FindUserIdsAsync(string claimType, string? claimValue, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a specific claim type and value from a user's index.
    /// </summary>
    /// <param name="claimType">The type of claim to remove.</param>
    /// <param name="claimValue">The value of the claim to remove.</param>
    /// <param name="userId">The ID of the user from whom to remove the claim.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method provides a way to remove an index entry by specifying the claim type and value separately,
    /// rather than passing a Claim object. The actual claim on the user's identity is not modified by this operation.
    /// </remarks>
    Task RemoveAsync(string claimType, string? claimValue, string userId, CancellationToken cancellationToken);
}