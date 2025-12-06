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

## Overview

This repository contains Hexalith Identity Stores components for user identity management, authentication, and authorization within the Hexalith ecosystem. It includes class libraries and an example app for integrating identity services into applications.

## Dapr Integration

[Dapr](https://dapr.io/) is used to provide building blocks around state, service invocation, pub/sub, and secrets. The libraries in this repo include helpers and actor-based stores designed to work with Dapr.

### Key Dapr Building Blocks Used

- State Management: Persist identity-related data using pluggable state stores.
- Service-to-Service Invocation: Communicate between identity actors/services and consumers.
- Pub/Sub Messaging: Publish identity-related events.
- Secrets Management: Store sensitive configuration securely.

### Dapr Architecture in Hexalith.IdentityStores

Identity data and operations are implemented using Dapr actors and services. Applications interact via standard Dapr APIs or client SDKs.

## Repository Structure

- `src/`: Core source code.
  - `src/libraries/`: Class libraries packaged as NuGet.
    - `Hexalith.IdentityStores` (library): Dapr actor-based identity stores, models, helpers, and services.
    - `Hexalith.IdentityStores.Abstractions` (library): Public abstractions, configurations, and constants shared across the ecosystem.
    - `Hexalith.IdentityStores.UI` (library): UI components and helpers (Blazor) for account flows.
  - `src/examples/`: Sample projects demonstrating usage.
    - `Hexalith.IdentityStores.Example` (console example): Minimal example wiring identity stores.
  - `src/servers/`: Documentation placeholder. No server projects are present in this repository at the moment. See `src/servers/README.md`.
- `test/`: Tests.
  - `Hexalith.IdentityStores.Tests`: Unit tests.
- `tools/`: Utility scripts.
- `Hexalith.Builds/`: Git submodule for shared build configurations.
- `.github/`: GitHub workflows and templates.
- Root files: Solution, props, package, license, and documentation.

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- PowerShell 7 or later
- Git
- Dapr (optional but recommended for actor/state features)
- Docker (optional, for running Dapr locally)

### Initialization

1. Clone the repository:
    ```bash
    git clone --recurse-submodules https://github.com/Hexalith/Hexalith.IdentityStores.git
    cd Hexalith.IdentityStores
    ```
    If you cloned without `--recurse-submodules`, run:
    ```bash
    git submodule update --init --recursive
    ```

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

Refer to contribution guidelines if available before submitting pull requests. Ensure code adheres to the project's coding standards and passes all tests.

## License

MIT License. See `LICENSE` for details.
