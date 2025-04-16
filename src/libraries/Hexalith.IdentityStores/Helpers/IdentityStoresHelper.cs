// <copyright file="IdentityStoresHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Helpers;

using System;

using Dapr.Actors.Runtime;

using Hexalith.Application.Sessions.Services;
using Hexalith.IdentityStores.Actors;
using Hexalith.IdentityStores.Services;
using Hexalith.Infrastructure.DaprRuntime.Actors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Provides helper methods for partition actors.
/// </summary>
public static class IdentityStoresHelper
{
    /// <summary>
    /// Adds Dapr identity store services to the specified server IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration containing the Dapr identity store settings.</param>
    /// <returns>The IServiceCollection with the added services.</returns>
    public static IServiceCollection AddIdentityStoresServer(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services
            .AddControllers()
                .AddDapr();
        services.TryAddSingleton<IUserCollectionService, UserCollectionService>();
        services.TryAddSingleton<IUserNameIndexService, UserNameIndexService>();
        services.TryAddSingleton<IUserEmailIndexService, UserEmailIndexService>();
        services.TryAddSingleton<IUserLoginIndexService, UserLoginIndexService>();
        services.TryAddSingleton<IUserClaimsIndexService, UserClaimsIndexService>();
        services.TryAddSingleton<IUserTokenIndexService, UserTokenIndexService>();
        services.TryAddSingleton<IRoleCollectionService, RoleCollectionService>();
        services.TryAddSingleton<IRoleNameIndexService, RoleNameIndexService>();
        services.TryAddSingleton<IRoleClaimsIndexService, RoleClaimsIndexService>();
        services.TryAddScoped<IUserPartitionService, UserPartitionService>();
        _ = services.AddIdentityStoresAuthentication(configuration);
        return services;
    }

    /// <summary>
    /// Registers partition actors with the specified ActorRegistrationCollection.
    /// </summary>
    /// <param name="actorRegistrationCollection">The ActorRegistrationCollection to register actors with.</param>
    /// <exception cref="ArgumentNullException">Thrown when actorRegistrationCollection is null.</exception>
    public static void RegisterIdentityActors(this ActorRegistrationCollection actorRegistrationCollection)
    {
        ArgumentNullException.ThrowIfNull(actorRegistrationCollection);
        actorRegistrationCollection.RegisterActor<UserActor>(IdentityStoresConstants.DefaultUserActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(IdentityStoresConstants.UserCollectionActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(IdentityStoresConstants.UserEmailIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(IdentityStoresConstants.UserNameIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(IdentityStoresConstants.UserLoginIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(IdentityStoresConstants.UserClaimIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(IdentityStoresConstants.UserTokenIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<RoleActor>(IdentityStoresConstants.DefaultRoleActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(IdentityStoresConstants.RoleCollectionActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyHashActor>(IdentityStoresConstants.RoleClaimIndexActorTypeName);
        actorRegistrationCollection.RegisterActor<KeyValueActor>(IdentityStoresConstants.RoleNameIndexActorTypeName);
    }
}