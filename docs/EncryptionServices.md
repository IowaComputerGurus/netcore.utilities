# Encryption Services

ICG.NetCore.Utilities provides two AES encryption services for different use cases.

## Overview

Both services use AES (Advanced Encryption Standard) symmetric encryption but differ in how they manage keys:

- **AesEncryptionService**: Uses pre-configured static key and IV values
- **AesDerivedKeyEncryptionService**: Derives keys from a passphrase and salt using PBKDF2 (RFC 2898)

## AesEncryptionService

Use this service when you have pre-generated encryption keys and want to use the same key/IV combination for all encryption operations.

### Configuration

```csharp
// In Startup.cs or Program.cs
services.Configure<AesEncryptionServiceOptions>(options =>
{
    options.Key = "your-base64-encoded-32-byte-key";
    options.IV = "your-base64-encoded-16-byte-iv";
});

// Don't forget to register the utilities
services.UseIcgNetCoreUtilities();
```

### Generating Keys

You can use the included EncryptionKeyGenerator utility or generate keys programmatically:

```csharp
using System.Security.Cryptography;

using (var aes = Aes.Create())
{
    aes.GenerateKey();
    aes.GenerateIV();
    
    var key = Convert.ToBase64String(aes.Key);
    var iv = Convert.ToBase64String(aes.IV);
    
    Console.WriteLine($"Key: {key}");
    Console.WriteLine($"IV: {iv}");
}
```

### Usage

```csharp
public class SecureDataService
{
    private readonly IAesEncryptionService _encryptionService;

    public SecureDataService(IAesEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public string EncryptSensitiveData(string plainText)
    {
        return _encryptionService.Encrypt(plainText);
    }

    public string DecryptSensitiveData(string encryptedText)
    {
        return _encryptionService.Decrypt(encryptedText);
    }
}
```

### Interface

```csharp
public interface IAesEncryptionService
{
    string Encrypt(string plainTextInput);
    string Decrypt(string encryptedInput);
}
```

### Use Cases

- Encrypting configuration values
- Protecting API keys in storage
- Encrypting database connection strings
- Any scenario where the same key is used across the application

### Security Considerations

- Store keys securely (use Azure Key Vault, AWS Secrets Manager, etc.)
- Never commit keys to source control
- Rotate keys periodically
- Use strong random keys (256-bit recommended)

## AesDerivedKeyEncryptionService

Use this service when you want to derive encryption keys from a passphrase and salt. This is ideal for per-user or per-record encryption where each item can have a unique salt.

### Configuration

```csharp
// In Startup.cs or Program.cs
services.Configure<AesDerivedKeyEncryptionServiceOptions>(options =>
{
    options.Passphrase = "your-secure-passphrase";
});

// Don't forget to register the utilities
services.UseIcgNetCoreUtilities();
```

### Usage with Configured Passphrase

```csharp
public class UserDataService
{
    private readonly IAesDerivedKeyEncryptionService _encryptionService;

    public UserDataService(IAesDerivedKeyEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void SaveUserData(int userId, string sensitiveData)
    {
        // Use user ID as salt for per-user encryption
        var salt = $"user_{userId}";
        var encrypted = _encryptionService.Encrypt(sensitiveData, salt);
        
        // Save encrypted data to database
        SaveToDatabase(userId, encrypted);
    }

    public string LoadUserData(int userId)
    {
        var encrypted = LoadFromDatabase(userId);
        var salt = $"user_{userId}";
        return _encryptionService.Decrypt(encrypted, salt);
    }
}
```

### Usage with Custom Passphrase

```csharp
public class DocumentEncryptionService
{
    private readonly IAesDerivedKeyEncryptionService _encryptionService;

    public DocumentEncryptionService(IAesDerivedKeyEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public EncryptedDocument EncryptDocument(string content, string userPassword)
    {
        // Generate a unique salt for this document
        var salt = Guid.NewGuid().ToString();
        
        // Use user's password as passphrase
        var encrypted = _encryptionService.Encrypt(content, salt, userPassword);
        
        return new EncryptedDocument
        {
            EncryptedContent = encrypted,
            Salt = salt  // Store salt with the document
        };
    }

    public string DecryptDocument(EncryptedDocument doc, string userPassword)
    {
        return _encryptionService.Decrypt(doc.EncryptedContent, doc.Salt, userPassword);
    }
}
```

### Interface

```csharp
public interface IAesDerivedKeyEncryptionService
{
    // Uses configured passphrase
    string Encrypt(string plainTextInput, string salt);
    string Decrypt(string encryptedInput, string salt);
    
    // Uses provided passphrase
    string Encrypt(string plainTextInput, string salt, string passphrase);
    string Decrypt(string encryptedInput, string salt, string passphrase);
}
```

### Use Cases

- Per-user data encryption
- Per-record encryption in databases
- Password-protected documents or files
- Multi-tenant applications where each tenant needs isolated encryption
- Scenarios requiring key rotation without re-encrypting all data

### Security Considerations

- Use unique salts for each encryption operation
- Store salts alongside encrypted data (they don't need to be secret)
- Use strong passphrases (long and complex)
- Consider using user-provided passwords for password-protected features
- Salt values should be unique but don't need to be cryptographically random

## Comparison

| Feature | AesEncryptionService | AesDerivedKeyEncryptionService |
|---------|---------------------|--------------------------------|
| Key Management | Static pre-generated keys | Keys derived from passphrase + salt |
| Configuration | Requires base64 key and IV | Requires passphrase only |
| Per-item Keys | No | Yes (via unique salts) |
| Performance | Faster (no key derivation) | Slightly slower (key derivation overhead) |
| Use Case | Application-wide encryption | Per-user or per-record encryption |
| Key Rotation | Requires re-encrypting all data | Can use different passphrases per operation |

## Best Practices

### General

1. **Never log or display encrypted values** in production
2. **Handle exceptions** from encryption/decryption operations gracefully
3. **Validate input** before encrypting to avoid wasting resources
4. **Use HTTPS** when transmitting encrypted data to prevent man-in-the-middle attacks

### AesEncryptionService

```csharp
// ✅ Good: Secure configuration
services.Configure<AesEncryptionServiceOptions>(options =>
{
    options.Key = Configuration["Encryption:Key"];  // From secure config
    options.IV = Configuration["Encryption:IV"];
});

// ❌ Bad: Hardcoded keys
services.Configure<AesEncryptionServiceOptions>(options =>
{
    options.Key = "hardcoded-key-in-source-code";  // DON'T DO THIS
});
```

### AesDerivedKeyEncryptionService

```csharp
// ✅ Good: Unique salt per record
public void EncryptUserData(User user)
{
    var salt = $"user_{user.Id}_{Guid.NewGuid()}";
    user.EncryptedData = _encryptionService.Encrypt(user.SensitiveData, salt);
    user.Salt = salt;  // Store with user
}

// ❌ Bad: Reusing same salt
public void EncryptUserData(User user)
{
    var salt = "same-salt-for-everyone";  // DON'T DO THIS
    user.EncryptedData = _encryptionService.Encrypt(user.SensitiveData, salt);
}
```

## Error Handling

```csharp
public class SafeEncryptionService
{
    private readonly IAesEncryptionService _encryptionService;
    private readonly ILogger<SafeEncryptionService> _logger;

    public string SafeEncrypt(string plainText)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
                
            return _encryptionService.Encrypt(plainText);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Null argument provided for encryption");
            throw;
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Encryption failed");
            throw new InvalidOperationException("Failed to encrypt data", ex);
        }
    }

    public string SafeDecrypt(string encryptedText)
    {
        try
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;
                
            return _encryptionService.Decrypt(encryptedText);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Null argument provided for decryption");
            throw;
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Invalid encrypted data format");
            throw new InvalidOperationException("Failed to decrypt data: invalid format", ex);
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Decryption failed");
            throw new InvalidOperationException("Failed to decrypt data", ex);
        }
    }
}
```

## Testing

### Unit Testing with Mock

```csharp
[Test]
public void Test_EncryptDecrypt_RoundTrip()
{
    // Arrange
    var options = Options.Create(new AesEncryptionServiceOptions
    {
        Key = "your-test-key-base64",
        IV = "your-test-iv-base64"
    });
    var service = new AesEncryptionService(options);
    var plainText = "sensitive data";

    // Act
    var encrypted = service.Encrypt(plainText);
    var decrypted = service.Decrypt(encrypted);

    // Assert
    Assert.AreNotEqual(plainText, encrypted);
    Assert.AreEqual(plainText, decrypted);
}
```

## Migration Between Services

If you need to migrate from AesEncryptionService to AesDerivedKeyEncryptionService:

```csharp
public class EncryptionMigrationService
{
    private readonly IAesEncryptionService _oldService;
    private readonly IAesDerivedKeyEncryptionService _newService;

    public void MigrateUserData(User user)
    {
        // Decrypt with old service
        var plainText = _oldService.Decrypt(user.EncryptedData);
        
        // Generate new salt
        var salt = Guid.NewGuid().ToString();
        
        // Encrypt with new service
        user.EncryptedData = _newService.Encrypt(plainText, salt);
        user.Salt = salt;
        user.EncryptionVersion = 2;  // Track encryption method
    }
}
```

## Additional Resources

- [AES Encryption (Wikipedia)](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard)
- [PBKDF2 (RFC 2898)](https://tools.ietf.org/html/rfc2898)
- [.NET Cryptography Model](https://docs.microsoft.com/en-us/dotnet/standard/security/cryptography-model)
