# ICG.NetCore.Utilities Documentation

Welcome to the comprehensive documentation for ICG.NetCore.Utilities.

## Quick Links

- [Main README](../README.md) - Overview, installation, and quick reference
- [QueryableExtensions](QueryableExtensions.md) - IQueryable extension methods for conditional filtering, ordering, and paging
- [EnumExtensions](EnumExtensions.md) - Enum extension methods for working with Display attributes
- [Encryption Services](EncryptionServices.md) - AES encryption services documentation

## What's Included

### Extension Methods

Extension methods that add functionality to existing .NET types:

- **[QueryableExtensions](QueryableExtensions.md)** - Conditional LINQ operations
  - WhereIf - Conditional filtering
  - OrderByIf / OrderByDescendingIf - Conditional ordering
  - GetPage - Pagination helper
  - DistinctBy - Distinct by key selector

- **[EnumExtensions](EnumExtensions.md)** - Display attribute helpers
  - GetDisplayNameOrStringValue - Safe display name retrieval
  - GetDisplayName - Display name retrieval (strict)
  - HasDisplayName - Check for Display attribute

- **IdentityExtensions** - Claims helper
  - GetClaimValue - Extract claim values from IIdentity

### Services

Services that provide testable implementations and utility functionality:

- **[Encryption Services](EncryptionServices.md)**
  - IAesEncryptionService - Static key encryption
  - IAesDerivedKeyEncryptionService - Passphrase-based encryption

- **Provider Interfaces** (for unit testing)
  - IDirectoryProvider - System.IO.Directory wrapper
  - IFileProvider - System.IO.File wrapper
  - IPathProvider - System.IO.Path wrapper
  - IGuidProvider - System.Guid wrapper
  - ITimeProvider - System.DateTime wrapper
  - ITimeSpanProvider - System.TimeSpan wrapper

- **Other Services**
  - IUrlSlugGenerator - URL-friendly slug generation
  - IDatabaseEnvironmentModelFactory - Connection string parsing

### Constants

- **Timezones** - Standard US timezone constants

## Getting Started

### Installation

```bash
Install-Package ICG.NetCore.Utilities
```

### Basic Setup

In your `Startup.cs` or `Program.cs`:

```csharp
services.UseIcgNetCoreUtilities();
```

### Basic Usage

```csharp
using ICG.NetCore.Utilities;

// Use QueryableExtensions
var users = dbContext.Users
    .WhereIf(activeOnly, u => u.IsActive)
    .OrderByIf(sortByName, u => u.Name)
    .GetPage(pageNumber, pageSize)
    .ToList();

// Use EnumExtensions
var displayName = myEnum.GetDisplayNameOrStringValue();

// Use IdentityExtensions
var email = User.Identity.GetClaimValue(ClaimTypes.Email);
```

## Documentation Organization

- **Main README**: High-level overview and quick reference for all features
- **Detailed Docs**: In-depth documentation for specific features with examples and best practices
  - [QueryableExtensions.md](QueryableExtensions.md) - Complete guide to IQueryable extensions
  - [EnumExtensions.md](EnumExtensions.md) - Complete guide to Enum extensions
  - [EncryptionServices.md](EncryptionServices.md) - Complete guide to encryption services

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for information on how to contribute to this project.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/IowaComputerGurus/netcore.utilities).

## Additional Resources

- [Source Code](https://github.com/IowaComputerGurus/netcore.utilities)
- [NuGet Package](https://www.nuget.org/packages/ICG.NetCore.Utilities/)
- [Release Notes](https://github.com/IowaComputerGurus/netcore.utilities/releases)
