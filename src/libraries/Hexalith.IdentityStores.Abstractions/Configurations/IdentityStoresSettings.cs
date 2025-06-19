// <copyright file="IdentityStoresSettings.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Configurations;

using Hexalith.Commons.Configurations;

/// <summary>
/// Security settings.
/// </summary>
/// <param name="microsoft">The Microsoft authentication credentials.</param>
/// <param name="github">The GitHub authentication credentials.</param>
/// <param name="google">The Google authentication credentials.</param>
/// <param name="facebook">The Facebook authentication credentials.</param>
/// <param name="x">The X authentication credentials.</param>
/// <param name="twitter">The Twitter authentication credentials.</param>
/// <param name="linkedIn">The LinkedIn authentication credentials.</param>
/// <param name="instagram">The Instagram authentication credentials.</param>
/// <param name="apple">The Apple authentication credentials.</param>
/// <param name="amazon">The Amazon authentication credentials.</param>
/// <param name="basecamp">The Basecamp authentication credentials.</param>
/// <param name="discord">The Discord authentication credentials.</param>
/// <param name="notion">The Notion authentication credentials.</param>
/// <param name="okta">The Okta authentication credentials.</param>
/// <param name="paypal">The PayPal authentication credentials.</param>
/// <param name="reddit">The Reddit authentication credentials.</param>
/// <param name="shopify">The Shopify authentication credentials.</param>
/// <param name="slack">The Slack authentication credentials.</param>
/// <param name="yahoo">The Yahoo authentication credentials.</param>
/// <param name="dataProtectionPath">The path to the data protection store.</param>
public class IdentityStoresSettings(
    AuthenticationCredentials? microsoft,
    AuthenticationCredentials? github,
    AuthenticationCredentials? google,
    AuthenticationCredentials? facebook,
    AuthenticationCredentials? x,
    AuthenticationCredentials? twitter,
    AuthenticationCredentials? linkedIn,
    AuthenticationCredentials? instagram,
    AuthenticationCredentials? apple,
    AuthenticationCredentials? amazon,
    AuthenticationCredentials? basecamp,
    AuthenticationCredentials? discord,
    AuthenticationCredentials? notion,
    AuthenticationCredentials? okta,
    AuthenticationCredentials? paypal,
    AuthenticationCredentials? reddit,
    AuthenticationCredentials? shopify,
    AuthenticationCredentials? slack,
    AuthenticationCredentials? yahoo,
    string? dataProtectionPath) : ISettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityStoresSettings"/> class.
    /// </summary>
    public IdentityStoresSettings()
        : this(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
    {
    }

    /// <summary>
    /// Gets or sets the Amazon authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Amazon { get; set; } = amazon;

    /// <summary>
    /// Gets or sets the Apple authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Apple { get; set; } = apple;

    /// <summary>
    /// Gets or sets the Basecamp authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Basecamp { get; set; } = basecamp;

    /// <summary>
    /// Gets or sets the Discord authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Discord { get; set; } = discord;

    /// <summary>
    /// Gets or sets the path to the data protection store.
    /// </summary>
    public string? DataProtectionPath { get; set; } = dataProtectionPath;

    /// <summary>
    /// Gets or sets the Facebook authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Facebook { get; set; } = facebook;

    /// <summary>
    /// Gets or sets the GitHub authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Github { get; set; } = github;

    /// <summary>
    /// Gets or sets the Google authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Google { get; set; } = google;

    /// <summary>
    /// Gets or sets the Instagram credentials.
    /// </summary>
    public AuthenticationCredentials? Instagram { get; set; } = instagram;

    /// <summary>
    /// Gets or sets the LinkedIn authentication credentials.
    /// </summary>
    public AuthenticationCredentials? LinkedIn { get; set; } = linkedIn;

    /// <summary>
    /// Gets or sets the Microsoft authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Microsoft { get; set; } = microsoft;

    /// <summary>
    /// Gets or sets the Notion authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Notion { get; set; } = notion;

    /// <summary>
    /// Gets or sets the Okta authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Okta { get; set; } = okta;

    /// <summary>
    /// Gets or sets the PayPal authentication credentials.
    /// </summary>
    public AuthenticationCredentials? PayPal { get; set; } = paypal;

    /// <summary>
    /// Gets or sets the Reddit authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Reddit { get; set; } = reddit;

    /// <summary>
    /// Gets or sets the Shopify authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Shopify { get; set; } = shopify;

    /// <summary>
    /// Gets or sets the Slack authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Slack { get; set; } = slack;

    /// <summary>
    /// Gets or sets the Twitter authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Twitter { get; set; } = twitter;

    /// <summary>
    /// Gets or sets the X authentication credentials.
    /// </summary>
    public AuthenticationCredentials? X { get; set; } = x;

    /// <summary>
    /// Gets or sets the Yahoo authentication credentials.
    /// </summary>
    public AuthenticationCredentials? Yahoo { get; set; } = yahoo;

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(IdentityStores);
}