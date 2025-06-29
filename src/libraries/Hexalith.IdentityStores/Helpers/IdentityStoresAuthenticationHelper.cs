// <copyright file="IdentityStoresAuthenticationHelper.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Helpers;

using System;
using System.IO;

using Hexalith.Commons.Configurations;
using Hexalith.IdentityStores.Configurations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            .Get<IdentityStoresSettings>()
                ?? new IdentityStoresSettings();

        // Get DOTNET_ReadOnlyDataProtectionKeyDirectory value from the configuration
        string readonlyDataProtectionKeyDirectory = configuration["DOTNET_ReadOnlyDataProtectionKeyDirectory"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(readonlyDataProtectionKeyDirectory))
        {
            SettingsException.ThrowIfUndefined<IdentityStoresSettings>(config.DataProtectionPath);

            // Ensure directory exists
            if (!Directory.Exists(config.DataProtectionPath))
            {
                try
                {
                    // Log the creation of the directory
                    Console.WriteLine($"Creating data protection directory at '{config.DataProtectionPath}'...");
                    _ = Directory.CreateDirectory(config.DataProtectionPath);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create data protection directory at '{config.DataProtectionPath}'. ", ex);
                }
            }

            // Add data protection with more reliable configuration
            _ = services.AddDataProtection()
                .SetApplicationName(nameof(Hexalith))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30))
                .PersistKeysToFileSystem(new DirectoryInfo(config.DataProtectionPath));
        }
        else
        {
            Console.WriteLine($"Running in Azure Container App with data protection enabled. Using read-only data protection directory at '{readonlyDataProtectionKeyDirectory}'...");
        }

        AuthenticationBuilder authentication = services
            .AddAuthentication()
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.SlidingExpiration = true;
            });

        if (config.Google?.Enabled == true)
        {
            authentication = authentication.AddGoogle(options =>
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
                    options.Scope.Add("read:user");
                    options.Scope.Add("user:email");
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

        if (config.LinkedIn?.Enabled == true)
        {
            _ = authentication.AddLinkedIn(options =>
                {
                    options.ClientId = config.LinkedIn.Id!;
                    options.ClientSecret = config.LinkedIn.Secret!;
                });
        }

        if (config.Instagram?.Enabled == true)
        {
            _ = authentication.AddInstagram(options =>
                {
                    options.ClientId = config.Instagram.Id!;
                    options.ClientSecret = config.Instagram.Secret!;
                });
        }

        if (config.Apple?.Enabled == true)
        {
            _ = authentication.AddApple(options =>
                {
                    options.ClientId = config.Apple.Id!;
                    options.ClientSecret = config.Apple.Secret!;
                });
        }

        if (config.Amazon?.Enabled == true)
        {
            _ = authentication.AddAmazon(options =>
                {
                    options.ClientId = config.Amazon.Id!;
                    options.ClientSecret = config.Amazon.Secret!;
                });
        }

        if (config.Basecamp?.Enabled == true)
        {
            _ = authentication.AddBasecamp(options =>
                {
                    options.ClientId = config.Basecamp.Id!;
                    options.ClientSecret = config.Basecamp.Secret!;
                });
        }

        if (config.Discord?.Enabled == true)
        {
            _ = authentication.AddDiscord(options =>
                {
                    options.ClientId = config.Discord.Id!;
                    options.ClientSecret = config.Discord.Secret!;
                });
        }

        if (config.Notion?.Enabled == true)
        {
            _ = authentication.AddNotion(options =>
                {
                    options.ClientId = config.Notion.Id!;
                    options.ClientSecret = config.Notion.Secret!;
                });
        }

        if (config.Okta?.Enabled == true)
        {
            _ = authentication.AddOkta(options =>
                {
                    options.ClientId = config.Okta.Id!;
                    options.ClientSecret = config.Okta.Secret!;
                });
        }

        if (config.PayPal?.Enabled == true)
        {
            _ = authentication.AddPaypal(options =>
                {
                    options.ClientId = config.PayPal.Id!;
                    options.ClientSecret = config.PayPal.Secret!;
                });
        }

        if (config.Reddit?.Enabled == true)
        {
            _ = authentication.AddReddit(options =>
                {
                    options.ClientId = config.Reddit.Id!;
                    options.ClientSecret = config.Reddit.Secret!;
                });
        }

        if (config.Shopify?.Enabled == true)
        {
            _ = authentication.AddShopify(options =>
                {
                    options.ClientId = config.Shopify.Id!;
                    options.ClientSecret = config.Shopify.Secret!;
                });
        }

        if (config.Slack?.Enabled == true)
        {
            _ = authentication.AddSlack(options =>
                {
                    options.ClientId = config.Slack.Id!;
                    options.ClientSecret = config.Slack.Secret!;
                });
        }

        if (config.Yahoo?.Enabled == true)
        {
            _ = authentication.AddYahoo(options =>
                {
                    options.ClientId = config.Yahoo.Id!;
                    options.ClientSecret = config.Yahoo.Secret!;
                });
        }

        return services;
    }
}