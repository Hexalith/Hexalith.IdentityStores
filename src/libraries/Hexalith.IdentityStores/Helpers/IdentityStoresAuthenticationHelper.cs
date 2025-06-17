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
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the specified certificate is not found.</exception>
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

        AuthenticationBuilder authentication = services.AddAuthentication();

        if (config.MicrosoftOidc?.Enabled == true)
        {
            // if (!string.IsNullOrWhiteSpace(config.MicrosoftOidc.CertificateThumbprint))
            // {
            //    using var store = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            //    store.Open(OpenFlags.ReadOnly);
            //    if (store.Certificates
            //        .Find(X509FindType.FindByThumbprint, config.MicrosoftOidc.CertificateThumbprint, false)
            //        .OfType<X509Certificate2>()
            //        .FirstOrDefault() == null)
            //    {
            //        throw new InvalidOperationException($"Certificate with thumbprint {config.MicrosoftOidc.CertificateThumbprint} not found in LocalMachine/My store.");
            //    }

            // msIdentityOptions.ClientCertificates =
            //    [
            //        new()
            //        {
            //            SourceType = CertificateSource.StoreWithThumbprint,
            //            CertificateStorePath = $"{store.Location}/{store.Name}",
            //            CertificateThumbprint = config.MicrosoftOidc.CertificateThumbprint,
            //        },
            //    ];
            // }
            // else if (!string.IsNullOrWhiteSpace(config.MicrosoftOidc.Secret))
            // {
            //    msIdentityOptions.ClientSecret = config.MicrosoftOidc.Secret;
            // }
            _ = authentication.AddOpenIdConnect(
                nameof(config.MicrosoftOidc),
                "Microsoft OIDC",
                options =>
            {
                options.ClientId = config.MicrosoftOidc.Id!;
                options.ClientSecret = config.MicrosoftOidc.Secret!;
                options.CallbackPath = string.IsNullOrWhiteSpace(config.MicrosoftOidc.CallbackPath)
                    ? "/signin-oidc"
                    : config.MicrosoftOidc.CallbackPath;
                options.Authority = $"https://login.microsoftonline.com/{config.MicrosoftOidc.Tenant}/v2.0";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.ValidateIssuer = false;
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = true;
                options.UseTokenLifetime = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });
        }
        else
        {
            authentication = authentication.AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.SlidingExpiration = true;
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