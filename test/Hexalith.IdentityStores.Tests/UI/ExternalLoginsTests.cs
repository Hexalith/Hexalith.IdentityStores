// <copyright file="ExternalLoginsTests.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.Tests.UI;

using System;
using System.Reflection;

using Hexalith.IdentityStores.UI.Account.Pages.Manage;

using Shouldly;

/// <summary>
/// Unit tests for the ExternalLogins component, focusing on the missing ExternalLoginExtensions class.
/// </summary>
public class ExternalLoginsTests
{
    /// <summary>
    /// Directly tests if the ExternalLoginExtensions type exists in the assembly.
    /// </summary>
    [Fact]
    public void ExternalLoginExtensionsShouldNotExistInAssembly()
    {
        // Arrange
        var identityStoresAssembly = Assembly.Load("Hexalith.IdentityStores");
        _ = identityStoresAssembly.ShouldNotBeNull("Assembly should be loaded");

        // Act
        bool typeExists = identityStoresAssembly.GetType("Hexalith.IdentityStores.Extensions.ExternalLoginExtensions") != null;

        // Assert
        typeExists.ShouldBeFalse("The type should not exist in the assembly");
    }

    /// <summary>
    /// Tests that a TypeLoadException is thrown when OnGetLinkLoginCallbackAsync method is invoked
    /// due to the missing ExternalLoginExtensions class.
    /// </summary>
    [Fact]
    public void OnGetLinkLoginCallbackAsyncShouldThrowTypeLoadException()
    {
        // Arrange
        var externalLogins = new ExternalLogins();

        // Use reflection to access the private method
        MethodInfo? methodInfo = typeof(ExternalLogins).GetMethod(
            "OnGetLinkLoginCallbackAsync",
            BindingFlags.NonPublic | BindingFlags.Instance);

        _ = methodInfo.ShouldNotBeNull("The OnGetLinkLoginCallbackAsync method should exist");

        // Act & Assert
        TypeLoadException exception = Should.Throw<TypeLoadException>(() =>
        {
            try
            {
                // Invoke the method
                // This will throw TargetInvocationException wrapping our TypeLoadException
                _ = methodInfo.Invoke(externalLogins, null);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is TypeLoadException)
            {
                // Unwrap and rethrow the TypeLoadException
                throw ex.InnerException;
            }
        });

        // Verify the exception is for the missing type
        exception.Message.ShouldContain("Hexalith.IdentityStores.Extensions.ExternalLoginExtensions");
    }
}