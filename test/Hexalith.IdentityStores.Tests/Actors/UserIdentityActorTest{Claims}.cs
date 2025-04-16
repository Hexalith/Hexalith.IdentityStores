﻿// <copyright file="UserIdentityActorTest{Claims}.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.IdentityStores.UnitTests.Actors;

using System.Security.Claims;

using Dapr.Actors.Runtime;

using Hexalith.IdentityStores.Actors;
using Hexalith.IdentityStores.Models;
using Hexalith.IdentityStores.Services;
using Hexalith.IdentityStores.States;
using Hexalith.Infrastructure.DaprRuntime.Helpers;

using Moq;

/// <summary>
/// Unit tests for the UserIdentityActor class which handles user identity operations.
/// Tests cover core functionality including user creation, deletion, and state management.
/// </summary>
public partial class UserIdentityActorTest
{
    /// <summary>
    /// Tests that AddClaimsAsync successfully adds new claims to an existing user
    /// and maintains uniqueness of claims.
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task AddClaimsAsyncShouldSucceed()
    {
        // Arrange
        // Create a test user identity with normalized username and email
        CustomUser user = User;

        // Create initial state with existing claims
        UserActorState initialState = new()
        {
            User = user,
            Claims =
            [
                new() { UserId = user.Id, ClaimType = "existing", ClaimValue = "value" }
            ],
        };

        // Create claims to add
        List<Claim> newClaims =
        [
            new("role", "admin"),
            new("permission", "read"),
        ];

        // Create mock state manager
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup state retrieval to return existing user with claims
        stateManagerMoq
            .Setup(p => p.TryGetStateAsync<UserActorState>(
                IdentityStoresConstants.UserStateName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConditionalValue<UserActorState>(true, initialState))
            .Verifiable();

        // Setup state update
        stateManagerMoq
            .Setup(p => p.SetStateAsync(
                IdentityStoresConstants.UserStateName,
                It.Is<UserActorState>(state =>
                    state.Claims.Count() == 3 &&
                    state.Claims.Any(c => c.ClaimType == "existing") &&
                    state.Claims.Any(c => c.ClaimType == "role") &&
                    state.Claims.Any(c => c.ClaimType == "permission")),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        stateManagerMoq
            .Setup(p => p.SaveStateAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Create service mocks
        Mock<IUserCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

        // Setup claim service mock for each new claim
        foreach (Claim claim in newClaims)
        {
            claimServiceMoq
                .Setup(p => p.AddAsync(
                    It.Is<string>(c => c == claim.Type),
                    It.Is<string>(c => c == claim.Value),
                    user.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        // Create actor host and actor
        var actorHost = ActorHost.CreateForTest<UserActor>(
            new ActorTestOptions { ActorId = user.Id.ToActorId() });

        UserActor actor = new(
            actorHost,
            collectionServiceMoq.Object,
            emailServiceMoq.Object,
            nameServiceMoq.Object,
            claimServiceMoq.Object,
            tokenServiceMoq.Object,
            loginServiceMoq.Object,
            stateManagerMoq.Object);

        // Act
        await actor.AddClaimsAsync(newClaims.Select(p => new CustomUserClaim { ClaimType = p.Type, ClaimValue = p.Value }));

        // Assert
        stateManagerMoq.Verify();
        claimServiceMoq.Verify();
    }

    /// <summary>
    /// Tests that GetClaimsAsync successfully retrieves all claims for a user.
    /// </summary>
    /// <returns>Task representing the test operation.</returns>
    [Fact]
    public async Task GetClaimsAsyncShouldReturnAllUserClaims()
    {
        // Arrange
        CustomUser user = User;

        // Create state with existing claims
        UserActorState state = new()
        {
            User = user,
            Claims =
            [
                new() { UserId = user.Id, ClaimType = "role", ClaimValue = "admin" },
                new() { UserId = user.Id, ClaimType = "permission", ClaimValue = "read" },
                new() { UserId = user.Id, ClaimType = "department", ClaimValue = "IT" }
            ],
        };

        // Create mock state manager
        Mock<IActorStateManager> stateManagerMoq = new(MockBehavior.Strict);

        // Setup state retrieval
        stateManagerMoq
            .Setup(p => p.TryGetStateAsync<UserActorState>(
                IdentityStoresConstants.UserStateName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConditionalValue<UserActorState>(true, state))
            .Verifiable();

        // Create service mocks
        Mock<IUserCollectionService> collectionServiceMoq = new(MockBehavior.Strict);
        Mock<IUserNameIndexService> nameServiceMoq = new(MockBehavior.Strict);
        Mock<IUserEmailIndexService> emailServiceMoq = new(MockBehavior.Strict);
        Mock<IUserClaimsIndexService> claimServiceMoq = new(MockBehavior.Strict);
        Mock<IUserTokenIndexService> tokenServiceMoq = new(MockBehavior.Strict);
        Mock<IUserLoginIndexService> loginServiceMoq = new(MockBehavior.Strict);

        // Create actor host and actor
        var actorHost = ActorHost.CreateForTest<UserActor>(
            new ActorTestOptions { ActorId = user.Id.ToActorId() });

        UserActor actor = new(
            actorHost,
            collectionServiceMoq.Object,
            emailServiceMoq.Object,
            nameServiceMoq.Object,
            claimServiceMoq.Object,
            tokenServiceMoq.Object,
            loginServiceMoq.Object,
            stateManagerMoq.Object);

        // Act
        IEnumerable<CustomUserClaim> claims = await actor.GetClaimsAsync();

        // Assert
        stateManagerMoq.Verify();
        Assert.NotNull(claims);
        Assert.Equal(3, claims.Count());
        Assert.Contains(claims, c => c.ClaimType == "role" && c.ClaimValue == "admin");
        Assert.Contains(claims, c => c.ClaimType == "permission" && c.ClaimValue == "read");
        Assert.Contains(claims, c => c.ClaimType == "department" && c.ClaimValue == "IT");
    }
}