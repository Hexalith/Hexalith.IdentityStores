// <copyright file="AuthenticationCredentials.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Configurations;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the authentication credentials.
/// </summary>
/// <param name="id">The identifier.</param>
/// <param name="secret">The secret.</param>
/// <param name="tenant">The Azure AD tenant ID for single-tenant applications.</param>
/// <param name="callbackPath">The callback path.</param>
public class AuthenticationCredentials(
    string? id,
    string? secret,
    string? tenant,
    string? callbackPath)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationCredentials"/> class.
    /// </summary>
    public AuthenticationCredentials()
        : this(null, null, null, null)
    {
    }

    /// <summary>
    /// Gets or sets the callback path.
    /// </summary>
    public string? CallbackPath { get; set; } = callbackPath;

    /// <summary>
    /// Gets a value indicating whether the credentials are enabled.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public bool Enabled => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Secret);

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string? Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the secret.
    /// </summary>
    public string? Secret { get; set; } = secret;

    /// <summary>
    /// Gets or sets the Azure AD tenant ID for single-tenant applications.
    /// </summary>
    public string? Tenant { get; set; } = tenant;
}