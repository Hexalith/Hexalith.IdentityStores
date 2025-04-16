// <copyright file="IdentityStoreUIExtensions.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.UI.Helpers;

using Hexalith.IdentityStore.UI.Services;
using Hexalith.IdentityStores.Helpers;
using Hexalith.IdentityStores.Models;
using Hexalith.IdentityStores.Stores;
using Hexalith.IdentityStores.UI.Account;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Provides extension methods for mapping additional identity endpoints required by the Identity Razor components.
/// </summary>
public static class IdentityStoreUIExtensions
{
    /// <summary>
    /// Adds the Dapr Identity Store UI services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration containing the Dapr Identity Store settings.</param>
    /// <returns>The IServiceCollection with the services added.</returns>
    public static IServiceCollection AddIdentityStoresUI(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<IUserClaimStore<CustomUser>, DaprActorUserStore>();
        services.TryAddScoped<IRoleClaimStore<CustomRole>, DaprActorRoleStore>();
        services.TryAddScoped<IUserStore<CustomUser>, DaprActorUserStore>();
        services.TryAddScoped<IRoleStore<CustomRole>, DaprActorRoleStore>();
        _ = services
            .AddIdentityStoresServer(configuration)
            .AddScoped<IEmailSender<CustomUser>, EmailSender>()
            .AddScoped<IdentityUserAccessor>()
            .AddScoped<IdentityRedirectManager>()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddIdentity<CustomUser, CustomRole>()
            .AddRoles<CustomRole>()
            .AddDefaultTokenProviders();
        return services;
    }
}