// <copyright file="IdentityStoresAuthenticationHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
    /// Adds Microsoft Entra OIDC authentication to the specified AuthenticationBuilder.
    /// </summary>
    /// <param name="authentication">The AuthenticationBuilder to add the OIDC authentication to.</param>
    /// <param name="credentials">The authentication credentials containing the OIDC settings.</param>
    /// <returns>The AuthenticationBuilder with the added OIDC authentication.</returns>
    public static AuthenticationBuilder AddEntraOidc(this AuthenticationBuilder authentication, AuthenticationCredentials credentials)
        => _ = authentication.AddOpenIdConnect(
                nameof(credentials),
                "Microsoft OIDC",
                options =>
            {
                options.ClientId = credentials.Id!;
                string tenant = credentials.Tenant?.Trim() ?? "common";

                // Use certificate if thumbprint is provided, otherwise use secret
                if (!string.IsNullOrWhiteSpace(credentials.CertificateThumbprint))
                {
                    X509Certificate2? cert = null;

                    // Try LocalMachine store first
                    using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                    {
                        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                        var certificates = store.Certificates
                            .Find(X509FindType.FindByThumbprint, credentials.CertificateThumbprint, false);

                        if (certificates.Count > 0)
                        {
                            cert = certificates[0];
                        }
                    }

                    // If not found in LocalMachine, try CurrentUser store
                    if (cert == null)
                    {
                        using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                        {
                            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                            var certificates = store.Certificates
                                .Find(X509FindType.FindByThumbprint, credentials.CertificateThumbprint, false);

                            if (certificates.Count > 0)
                            {
                                cert = certificates[0];
                            }
                        }
                    }

                    if (cert == null)
                    {
                        throw new InvalidOperationException($"Certificate with thumbprint '{credentials.CertificateThumbprint}' not found in either LocalMachine/My or CurrentUser/My certificate stores.");
                    }

                    if (!cert.HasPrivateKey)
                    {
                        throw new InvalidOperationException($"Certificate with thumbprint '{credentials.CertificateThumbprint}' does not have a private key. Please ensure the certificate was imported with its private key and that the application has permission to access it.");
                    }

                    // Use certificate for client authentication (do not set ClientSecret when using certificate)
                    var tokenHandler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
                    string clientId = credentials.Id!;

                    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                    {
                        OnAuthorizationCodeReceived = context =>
                        {
                            context.TokenEndpointRequest?.SetParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                            context.TokenEndpointRequest?.SetParameter("client_assertion", tokenHandler.CreateToken(
                                new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                                {
                                    Issuer = clientId,
                                    Audience = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
                                    Claims = new Dictionary<string, object>
                                    {
                                        { "sub", clientId },
                                        { "jti", Guid.NewGuid().ToString() },
                                    },
                                    Expires = DateTime.UtcNow.AddMinutes(5),
                                    SigningCredentials = new Microsoft.IdentityModel.Tokens.X509SigningCredentials(cert),
                                }));
                            return Task.CompletedTask;
                        },
                    };
                }
                else if (!string.IsNullOrWhiteSpace(credentials.Secret))
                {
                    options.ClientSecret = credentials.Secret!;
                }

                options.CallbackPath = string.IsNullOrWhiteSpace(credentials.CallbackPath)
                    ? "/signin-oidc"
                    : credentials.CallbackPath;
                options.Authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
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
            authentication = authentication.AddEntraOidc(config.MicrosoftOidc);
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