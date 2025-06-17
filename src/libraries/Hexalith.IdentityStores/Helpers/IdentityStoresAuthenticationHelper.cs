// <copyright file="IdentityStoresAuthenticationHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Helpers;

using System;

using Hexalith.IdentityStores.Configurations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

/// <summary>
/// Provides helper methods for partition actors.
/// </summary>
public static class IdentityStoresAuthenticationHelper
{
    /// <summary>
    /// Adds Dapr identity store authentication to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add authentication to.</param>
    /// <param name="configuration">The configuration containing the Dapr identity store settings.</param>
    /// <returns>The IServiceCollection with the added authentication services.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "Not complex")]
    public static IServiceCollection AddIdentityStoresAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        IdentityStoresSettings? config = configuration.GetSection(IdentityStoresSettings
            .ConfigurationName())
            .Get<IdentityStoresSettings>();
        if (config is null)
        {
            return services;
        }

        AuthenticationBuilder authentication = services.AddAuthentication().AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.SlidingExpiration = true;
        });

        if (config.MicrosoftOidc?.Enabled == true)
        {
            authentication = authentication.AddOpenIdConnect(nameof(config.MicrosoftOidc), options =>
            {
                // Core endpoints ----------------------------------------------------------------
                string tenant = string.IsNullOrWhiteSpace(config.MicrosoftOidc.Tenant)
                                    ? "common"
                                    : config.MicrosoftOidc.Tenant;
                options.Authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
                options.MetadataAddress = $"{options.Authority}/.well-known/openid-configuration";

                // App registration credentials --------------------------------------------------
                options.ClientId = config.MicrosoftOidc.Id!;
                options.ClientSecret = config.MicrosoftOidc.Secret;         // nullable for public clients

                // Protocol details --------------------------------------------------------------
                options.ResponseType = OpenIdConnectResponseType.Code;      // Auth‑code flow (OIDC/OAuth2)
                options.SaveTokens = true;                                // Persist id_token & access_token in auth cookie
                options.CallbackPath = string.IsNullOrWhiteSpace(config.MicrosoftOidc.CallbackPath)
                                            ? "/signin-oidc" // default
                                            : config.MicrosoftOidc.CallbackPath;

                // Requested scopes --------------------------------------------------------------
                options.Scope.Clear();                                      // start from scratch
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // options.Scope.Add("offline_access");                    // refresh tokens (web apps)
                // options.Scope.Add("api://<API‑App‑Id>/access_as_user"); // call downstream API

                // Claim handling ----------------------------------------------------------------
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.ValidateIssuer = false;   // for multi‑tenant – tighten for single tenant

                // Optional: map extra claims from id_token
                options.MapInboundClaims = false;   // keep original claim types
            });
        }

        if (config.Google?.Enabled == true)
        {
            authentication = authentication.AddGoogleOpenIdConnect(options =>
                {
                    options.ClientId = config.Google.Id!;
                    options.ClientSecret = config.Google.Secret!;
                });
        }

        if (config.Microsoft?.Enabled == true)
        {
            authentication = authentication.AddMicrosoftAccount(options =>
                {
                    options.ClientId = config.Microsoft.Id!;
                    options.ClientSecret = config.Microsoft.Secret!;

                    // Set tenant-specific configuration if provided
                    if (!string.IsNullOrEmpty(config.Microsoft.Tenant))
                    {
                        options.AuthorizationEndpoint = $"https://login.microsoftonline.com/{config.Microsoft.Tenant}/oauth2/v2.0/authorize";
                        options.TokenEndpoint = $"https://login.microsoftonline.com/{config.Microsoft.Tenant}/oauth2/v2.0/token";
                    }
                });
        }

        if (config.Github?.Enabled == true)
        {
            authentication = authentication.AddGitHub(options =>
                {
                    options.ClientId = config.Github.Id!;
                    options.ClientSecret = config.Github.Secret!;
                });
        }

        if (config.Facebook?.Enabled == true)
        {
            authentication = authentication.AddFacebook(options =>
                {
                    options.AppId = config.Facebook.Id!;
                    options.AppSecret = config.Facebook.Secret!;
                });
        }

        if (config.X?.Enabled == true)
        {
            _ = authentication.AddTwitter(options =>
                {
                    options.ClientId = config.X.Id!;
                    options.ClientSecret = config.X.Secret!;
                });
        }

        return services;
    }
}