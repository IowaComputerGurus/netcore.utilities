# netcore.utilities ![](https://img.shields.io/github/license/iowacomputergurus/netcore.utilities.svg)

A collection of helpful utilities for working with .NET + projects.  These items are used by the IowaComputerGurus Team to aid in unit testing and other common tasks

![Build Status](https://github.com/IowaComputerGurus/netcore.utilities/actions/workflows/ci-build.yml/badge.svg)

## NuGet Package Information
ICG.NetCore.Utilities ![](https://img.shields.io/nuget/v/icg.netcore.utilities.svg) ![](https://img.shields.io/nuget/dt/icg.netcore.utilities.svg)

## SonarCloud Analysis

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=IowaComputerGurus_netcore.utilities&metric=alert_status)](https://sonarcloud.io/dashboard?id=IowaComputerGurus_netcore.utilities)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=IowaComputerGurus_netcore.utilities&metric=coverage)](https://sonarcloud.io/dashboard?id=IowaComputerGurus_netcore.utilities)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=IowaComputerGurus_netcore.utilities&metric=security_rating)](https://sonarcloud.io/dashboard?id=IowaComputerGurus_netcore.utilities)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=IowaComputerGurus_netcore.utilities&metric=sqale_index)](https://sonarcloud.io/dashboard?id=IowaComputerGurus_netcore.utilities)

## Documentation

📚 **[View Detailed Documentation](docs/README.md)** - Comprehensive guides with examples and best practices

Quick links:
- [QueryableExtensions](docs/QueryableExtensions.md) - Conditional LINQ operations
- [EnumExtensions](docs/EnumExtensions.md) - Display attribute helpers
- [Encryption Services](docs/EncryptionServices.md) - AES encryption services

## Usage

### Installation

Install from NuGet

```
Install-Package ICG.NetCore.Utilities
```

### Register Dependencies

Inside of of your project's Startus.cs within the RegisterServices method add this line of code.

```
services.UseIcgNetCoreUtilities();
```

### Included C# Objects and Services

#### Provider Interfaces (for Testability)

| Object | Purpose |
| ---- | --- |
| IDirectoryProvider | Provides a shim around the System.IO.Directory object to allow for unit testing. |
| IGuidProvider | Provides a shim around the System.Guid object to allow for unit testing of Guid operations.  |
| IFileProvider | Provides a shim around the System.IO.File object to allow for unit testing of file related operations. |
| IPathProvider | Provides a shim around the System.IO.Path object to allow for unit testing of path related operations | 
| ITimeProvider | Provides a shim around the System.DateTime object to allow for unit testing of date operations |
| ITimeSpanProvider | Provides a shim around the System.TimeSpan object to allow for unit testing/injection of TimeSpan operations |

#### Services

| Service | Purpose |
| ---- | --- |
| IUrlSlugGenerator | Provides a service that will take input and generate a url friendly slug from the content |
| IAesEncryptionService | Provides a service that will encrypt and decrypt provided strings using AES symmetric encryption with configured key and IV |
| IAesDerivedKeyEncryptionService | Provides a service that will encrypt and decrypt provided strings using AES encryption with derived keys from passphrase and salt |
| IDatabaseEnvironmentModelFactory | Factory to create DatabaseEnvironmentModel objects from connection strings for display purposes |

#### Extension Methods

| Extension Class | Purpose |
| ---- | --- |
| QueryableExtensions | Extension methods for IQueryable<T> to simplify conditional filtering, ordering, paging, and distinct selection |
| EnumExtensions | Extension methods for Enum types to help display formatted enum values with Display attributes |
| IdentityExtensions | Extension methods for IIdentity objects to extract claim values |

#### Constants

| Class | Purpose |
| ---- | --- |
| Timezones | Helper constants for standard US timezone values |

## Detailed Documentation

### QueryableExtensions

Extension methods for `IQueryable<T>` that provide conditional querying capabilities.

**Available Methods:**

- **WhereIf** - Conditionally applies a filter to the query
  ```csharp
  var results = dbContext.Users
      .WhereIf(filterByActive, u => u.IsActive)
      .WhereIf(!string.IsNullOrEmpty(searchTerm), u => u.Name.Contains(searchTerm));
  ```

- **OrderByIf** - Conditionally applies ascending order to the query
  ```csharp
  var results = dbContext.Users
      .OrderByIf(sortByName, u => u.Name);
  ```

- **OrderByDescendingIf** - Conditionally applies descending order to the query
  ```csharp
  var results = dbContext.Users
      .OrderByDescendingIf(sortByNewest, u => u.CreatedDate);
  ```

- **GetPage** - Returns a specific page of results (1-based page numbers)
  ```csharp
  var results = dbContext.Users
      .OrderBy(u => u.Name)
      .GetPage(pageNumber: 2, pageSize: 10);
  ```

- **DistinctBy** - Returns distinct elements by a specified key selector
  ```csharp
  var uniqueUsers = dbContext.Users
      .DistinctBy(u => u.Email);
  ```

### EnumExtensions

Extension methods for working with Enum types and their Display attributes.

**Available Methods:**

- **GetDisplayNameOrStringValue** - Returns the Display Name attribute value or the enum's string value
  ```csharp
  public enum Status
  {
      [Display(Name = "Active User")]
      Active,
      Inactive
  }
  
  var displayName = Status.Active.GetDisplayNameOrStringValue(); // "Active User"
  var defaultName = Status.Inactive.GetDisplayNameOrStringValue(); // "Inactive"
  ```

- **GetDisplayName** - Gets the Display Name attribute value (throws if not found)
  ```csharp
  var displayName = Status.Active.GetDisplayName(); // "Active User"
  ```

- **HasDisplayName** - Checks if an enum value has a Display attribute
  ```csharp
  var hasDisplay = Status.Active.HasDisplayName(); // true
  var hasDisplay = Status.Inactive.HasDisplayName(); // false
  ```

### IdentityExtensions

Extension methods for working with IIdentity and claims.

**Available Methods:**

- **GetClaimValue** - Extracts the value of a specific claim type from a ClaimsIdentity
  ```csharp
  // In a Razor view or controller
  var firstName = User.Identity.GetClaimValue("Profile:FirstName");
  var email = User.Identity.GetClaimValue(ClaimTypes.Email);
  ```

### AesEncryptionService

Service for AES symmetric encryption with pre-configured key and IV values.

**Configuration:**
```csharp
services.Configure<AesEncryptionServiceOptions>(options =>
{
    options.Key = "your-base64-encoded-key";
    options.IV = "your-base64-encoded-iv";
});
```

**Usage:**
```csharp
public class MyService
{
    private readonly IAesEncryptionService _encryptionService;
    
    public MyService(IAesEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }
    
    public void EncryptData()
    {
        var encrypted = _encryptionService.Encrypt("sensitive data");
        var decrypted = _encryptionService.Decrypt(encrypted);
    }
}
```

### AesDerivedKeyEncryptionService

Service for AES encryption using derived keys from a passphrase and salt (using Rfc2898DeriveBytes).

**Configuration:**
```csharp
services.Configure<AesDerivedKeyEncryptionServiceOptions>(options =>
{
    options.Passphrase = "your-secure-passphrase";
});
```

**Usage:**
```csharp
public class MyService
{
    private readonly IAesDerivedKeyEncryptionService _encryptionService;
    
    public MyService(IAesDerivedKeyEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }
    
    public void EncryptData()
    {
        var salt = "unique-salt-value";
        var encrypted = _encryptionService.Encrypt("sensitive data", salt);
        var decrypted = _encryptionService.Decrypt(encrypted, salt);
    }
}
```

### DatabaseEnvironmentModelFactory

Factory for creating DatabaseEnvironmentModel objects from connection strings.

**Usage:**
```csharp
public class MyService
{
    private readonly IDatabaseEnvironmentModelFactory _factory;
    
    public MyService(IDatabaseEnvironmentModelFactory factory)
    {
        _factory = factory;
    }
    
    public void DisplayDbInfo()
    {
        var connectionString = "Server=myserver;Database=mydb;...";
        var model = _factory.CreateFromConnectionString("MyDatabase", connectionString);
        
        Console.WriteLine($"Server: {model.ServerName}");
        Console.WriteLine($"Database: {model.DatabaseName}");
    }
}
```

### Timezones

Static class containing constants for standard US timezone values.

**Available Constants:**
- `Timezones.AlaskanStandardTime`
- `Timezones.AtlanticStandardTime`
- `Timezones.CentralStandardTime`
- `Timezones.EasternStandardTime`
- `Timezones.HawaiianStandardTime`
- `Timezones.PacificStandardTime`
- `Timezones.MountainStandardTime`

**Usage:**
```csharp
var centralTime = TimeZoneInfo.FindSystemTimeZoneById(Timezones.CentralStandardTime);
var convertedTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, centralTime);
```

## Additional Information

For more detailed information, please refer to the XML documentation comments in the source code. All public APIs are fully documented with parameter descriptions, return values, and usage examples.

