# Hexalith.IdentityStores

[![License: MIT](https://img.shields.io/github/license/hexalith/hexalith.IdentityStores)](https://github.com/hexalith/hexalith/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1063152441819942922?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discordapp.com/channels/1102166958918610994/1102166958918610997)

## Build Status

[![Coverity Scan Build Status](https://scan.coverity.com/projects/31529/badge.svg)](https://scan.coverity.com/projects/hexalith-hexalith-IdentityStores)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/5c786c54c5a9494aa3baa9991ef572dc)](https://app.codacy.com/gh/Hexalith/Hexalith.IdentityStores/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.IdentityStores&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.IdentityStores)

[![Build status](https://github.com/Hexalith/Hexalith.IdentityStores/actions/workflows/build-release.yml/badge.svg)](https://github.com/Hexalith/Hexalith.IdentityStores/actions)
[![NuGet](https://img.shields.io/nuget/v/Hexalith.IdentityStores.svg)](https://www.nuget.org/packages/Hexalith.IdentityStores) <!-- Assuming a main package exists -->
[![Latest](https://img.shields.io/github/v/release/Hexalith/Hexalith.IdentityStores?include_prereleases&label=preview)](https://github.com/Hexalith/Hexalith.IdentityStores/pkgs/nuget/Hexalith.IdentityStores) <!-- Adjust if main package name differs -->

## Overview

This repository contains the Hexalith Dapr Identity Stores components, providing functionalities related to user identity management, authentication, and authorization within the Hexalith ecosystem. It includes libraries, server implementations, and examples for integrating identity services into applications.

## Dapr Integration

[Dapr](https://dapr.io/) (Distributed Application Runtime) is a portable, event-driven runtime that makes it easy to build resilient, microservice-based applications. Hexalith.IdentityStores leverages Dapr's capabilities in the following ways:

### Key Dapr Building Blocks Used

- **State Management**: Identity data is persisted using Dapr's state management, allowing for pluggable storage providers (Redis, Azure CosmosDB, etc.)
- **Service-to-Service Invocation**: Secure communication between identity services and consumers
- **Pub/Sub Messaging**: Event-driven architecture for identity-related events (user created, role changed, etc.)
- **Secrets Management**: Secure storage of sensitive identity configuration 

### Benefits of Dapr in Hexalith.IdentityStores

- **Platform Agnostic**: Runs on Kubernetes, VMs, or local development environments
- **Pluggable Components**: Easily swap underlying infrastructure (databases, message queues) without code changes
- **Language Independence**: Interact with identity services from any language or framework that supports HTTP/gRPC
- **Built-in Resilience**: Circuit breaking, retries, and distributed tracing capabilities

### Dapr Architecture in Hexalith.IdentityStores

Identity data is stored in Dapr state stores while the authentication and authorization logic is implemented as Dapr services. Applications can interact with the identity services through standard Dapr APIs, using either the Dapr client SDKs or direct HTTP/gRPC calls.

## Repository Structure

The repository is organized as follows:

- **`src/`**: Contains the core source code.
  - [`src/libraries/`](./src/libraries/README.md): Class libraries intended to be packaged as NuGet packages. This is likely where the main identity store logic resides.
  - [`src/servers/`](./src/servers/README.md): Server projects, potentially including API endpoints or services related to identity, possibly packaged as Docker containers.
  - [`src/examples/`](./src/examples/README.md): Sample projects demonstrating how to use the libraries and servers.
- **`test/`**: Contains unit, integration, and other test projects.
  - [`test/Hexalith.IdentityStores.Tests/`](./test/Hexalith.IdentityStores.Tests/): Example test project.
- **`tools/`**: Utility scripts for development tasks (e.g., `flush_redis.bat`).
- **`Hexalith.Builds/`**: Git submodule containing shared build configurations, tools, and scripts. See [Hexalith.Builds/README.md](./Hexalith.Builds/README.md) for details on the build system.
- **`.github/`**: GitHub-specific files, including workflow definitions and issue templates.
- **Root Files**: Configuration files (`.sln`, `.props`, `package.json`), license, documentation instructions (`DOCUMENTATION.ai.md`), and this README.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later
- [PowerShell 7](https://github.com/PowerShell/PowerShell) or later (for initialization script)
- [Git](https://git-scm.com/) (including Git LFS if used)
- [Dapr](https://dapr.io/) 
- [Docker](https://www.docker.com/) (if running server projects)

### Initialization

1.  **Clone the repository:**
    ```bash
    git clone --recurse-submodules https://github.com/Hexalith/Hexalith.IdentityStores.git
    cd Hexalith.IdentityStores
    ```
    If you cloned without `--recurse-submodules`, run `git submodule update --init --recursive`.

### Building the Code

Build the entire solution using the .NET CLI:

```bash
dotnet build Hexalith.IdentityStores.sln
```

## Development

### Running Tests

Execute tests using the .NET CLI:

```bash
dotnet test Hexalith.IdentityStores.sln
```

### Contributing

Please refer to the contribution guidelines (if available) before submitting pull requests. Ensure code adheres to the project's coding standards and passes all tests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
