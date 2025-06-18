// <copyright file="IdentityStoresAuthenticationHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using Hexalith.IdentityStores.Configurations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
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
    /// <exception cref="InvalidOperationException">Thrown when the certificate thumbprint is not provided, when the certificate is not found in LocalMachine/My store, or when the certificate does not have a private key.</exception>
    public static MicrosoftIdentityWebAppAuthenticationBuilder AddEntraOidc(this AuthenticationBuilder authentication, AuthenticationCredentials credentials)
    {
        if (string.IsNullOrWhiteSpace(credentials.CertificateThumbprint))
        {
            throw new InvalidOperationException("Certificate thumbprint must be provided for Microsoft OIDC authentication.");
        }

        // Verify certificate exists and is accessible
        X509Certificate2? cert = null;
        using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
        {
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificates = store.Certificates
                .Find(X509FindType.FindByThumbprint, credentials.CertificateThumbprint, false);

            if (certificates.Count > 0)
            {
                cert = certificates[0];
            }
        }

        if (cert == null)
        {
            throw new InvalidOperationException($"Certificate with thumbprint '{credentials.CertificateThumbprint}' not in LocalMachine/My.");
        }

        if (!cert.HasPrivateKey)
        {
            throw new InvalidOperationException($"Certificate with thumbprint '{credentials.CertificateThumbprint}' does not have a private key. Please ensure the certificate was imported with its private key and that the application has permission to access it.");
        }

        // Validate certificate with more detailed error reporting
        bool isValid = cert.Verify();
        if (!isValid)
        {
            // Check specific validation issues
            var validationErrors = new List<string>();

            // Check if certificate is expired
            if (DateTime.Now < cert.NotBefore)
            {
                validationErrors.Add($"Certificate is not yet valid. Valid from: {cert.NotBefore}");
            }
            else if (DateTime.Now > cert.NotAfter)
            {
                validationErrors.Add($"Certificate has expired. Valid until: {cert.NotAfter}");
            }

            // Check if it's self-signed
            if (cert.Issuer == cert.Subject)
            {
                validationErrors.Add("Certificate is self-signed");
            }

            // In development environments, we might want to allow invalid certificates
            bool allowInvalidCertificates = credentials.AllowInvalidCertificates ?? false;

            if (!allowInvalidCertificates)
            {
                string errorDetails = validationErrors.Count > 0
                    ? $" Issues found: {string.Join(", ", validationErrors)}"
                    : " The certificate chain could not be validated.";

                throw new InvalidOperationException(
                    $"Certificate with thumbprint '{credentials.CertificateThumbprint}' is not valid.{errorDetails} " +
                    "If this is a development environment with a self-signed certificate, set 'AllowInvalidCertificates' to true in your configuration.");
            }
        }

        _ = cert.GetRSAPrivateKey() ?? throw new InvalidOperationException($"Certificate with thumbprint '{credentials.CertificateThumbprint}' does not have a valid RSA private key. Please ensure the certificate was imported with its private key and that the application has permission to access it.");

        string tenant = credentials.Tenant?.Trim() ?? throw new InvalidOperationException("Tenant must be provided for Microsoft OIDC authentication.");

        // Use the configuration-based approach
        return authentication.AddMicrosoftIdentityWebApp(
            options =>
            {
                options.ClientId = credentials.Id!;
                options.TenantId = tenant;
                options.Instance = "https://login.microsoftonline.com/";
                options.CallbackPath = string.IsNullOrWhiteSpace(credentials.CallbackPath)
                    ? "/signin-oidc"
                    : credentials.CallbackPath;
                options.Authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.SaveTokens = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Send the public certificate (x5c) in the token request so Azure AD can
                // validate the client assertion without requiring the certificate to be
                // pre-uploaded in the app registration. This avoids AADSTS50146 errors when
                // using hybrid flow (code + id_token).
                options.SendX5C = true;

                // Configure the certificate in the client credentials
                options.ClientCredentials = [CertificateDescription.FromStoreWithThumbprint(
                    credentials.CertificateThumbprint,
                    StoreLocation.LocalMachine,
                    StoreName.My)
                ];
            },
            subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true,
            displayName: "Microsoft OIDC");
    }

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

        // Configure data protection to prevent state validation errors
        _ = services.AddDataProtection()
            .SetApplicationName("HexalithIdentityStores")
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HexalithIdentityStores", "DataProtection-Keys")));

        AuthenticationBuilder authentication = services.AddAuthentication();

        if (config.MicrosoftOidc?.Enabled == true)
        {
            _ = authentication.AddEntraOidc(config.MicrosoftOidc);
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
            authentication = authentication.AddGoogle(options =>
                {
                    options.ClientId = config.Google.Id!;
                    options.ClientSecret = config.Google.Secret!;
                    options.CallbackPath = string.IsNullOrWhiteSpace(config.Google.CallbackPath)
                        ? null
                        : config.Google.CallbackPath;
                    options.SaveTokens = true;
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