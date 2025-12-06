# EnumExtensions

Extension methods for Enum types that help display formatted enum values using Display attributes.

## Namespace

```csharp
using ICG.NetCore.Utilities;
using System.ComponentModel.DataAnnotations;
```

## Overview

The EnumExtensions class provides extension methods that make it easier to work with enum values and their Display attributes. This is particularly useful when you need to show user-friendly names for enum values in UI applications.

## Methods

### GetDisplayNameOrStringValue

Returns the configured Display Name, or default string value for the given enum value. This is the safest method as it will never throw an exception.

**Signature:**
```csharp
public static string GetDisplayNameOrStringValue(this Enum enumValue)
```

**Parameters:**
- `enumValue`: The enum value to get the display name for

**Returns:** The Display attribute's Name property if it exists, otherwise the enum's ToString() value

**Example:**
```csharp
public enum UserStatus
{
    [Display(Name = "Active User")]
    Active,
    
    [Display(Name = "Temporarily Inactive")]
    Inactive,
    
    Pending  // No Display attribute
}

var activeDisplay = UserStatus.Active.GetDisplayNameOrStringValue(); 
// Returns: "Active User"

var inactiveDisplay = UserStatus.Inactive.GetDisplayNameOrStringValue(); 
// Returns: "Temporarily Inactive"

var pendingDisplay = UserStatus.Pending.GetDisplayNameOrStringValue(); 
// Returns: "Pending"
```

### GetDisplayName

Gets the display name of an enum value. This method will throw a NullReferenceException if the Display attribute is not found.

**Signature:**
```csharp
public static string GetDisplayName(this Enum enumValue)
```

**Parameters:**
- `enumValue`: The enum value to get the display name for

**Returns:** The Display attribute's Name property

**Throws:** `NullReferenceException` when the Display attribute is not found

**Example:**
```csharp
public enum Priority
{
    [Display(Name = "Low Priority")]
    Low,
    
    [Display(Name = "Medium Priority")]
    Medium,
    
    [Display(Name = "High Priority")]
    High
}

var display = Priority.High.GetDisplayName(); 
// Returns: "High Priority"

// This would throw NullReferenceException:
// var display = SomeEnumWithoutDisplayAttribute.Value.GetDisplayName();
```

**Use Case:** Use this method when you know all enum values have Display attributes and you want to fail fast if one is missing.

### HasDisplayName

Checks whether an enum value has a Display attribute.

**Signature:**
```csharp
public static bool HasDisplayName(this Enum enumValue)
```

**Parameters:**
- `enumValue`: The enum value to check

**Returns:** `true` if the enum value has a Display attribute; otherwise `false`

**Example:**
```csharp
public enum OrderStatus
{
    [Display(Name = "Order Pending")]
    Pending,
    
    Shipped  // No Display attribute
}

var hasPendingDisplay = OrderStatus.Pending.HasDisplayName(); 
// Returns: true

var hasShippedDisplay = OrderStatus.Shipped.HasDisplayName(); 
// Returns: false

// Conditional display logic:
var displayValue = OrderStatus.Shipped.HasDisplayName() 
    ? OrderStatus.Shipped.GetDisplayName() 
    : OrderStatus.Shipped.ToString();
```

## Real-World Usage Scenarios

### Displaying Enum Values in a Dropdown

```csharp
// ASP.NET Core Razor View
@model IEnumerable<UserStatus>

<select name="status">
    @foreach (var status in Enum.GetValues<UserStatus>())
    {
        <option value="@status">@status.GetDisplayNameOrStringValue()</option>
    }
</select>
```

### API Response with Friendly Names

```csharp
public class OrderDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string StatusDisplay { get; set; }
}

public OrderDto MapToDto(Order order)
{
    return new OrderDto
    {
        Id = order.Id,
        Status = order.Status.ToString(),
        StatusDisplay = order.Status.GetDisplayNameOrStringValue()
    };
}
```

### Building a Dynamic Table

```csharp
public class EnumTableBuilder
{
    public List<EnumDisplayItem> GetEnumDisplayItems<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value => new EnumDisplayItem
            {
                Value = Convert.ToInt32(value),
                Name = value.ToString(),
                DisplayName = value.GetDisplayNameOrStringValue(),
                HasCustomDisplay = value.HasDisplayName()
            })
            .ToList();
    }
}
```

### Validation Messages with Display Names

```csharp
public class StatusValidator
{
    public string ValidateStatus(UserStatus status)
    {
        if (status == UserStatus.Inactive)
        {
            return $"Cannot process: Status is {status.GetDisplayNameOrStringValue()}";
        }
        return "Valid";
    }
}
```

## Best Practices

### When to Use Each Method

1. **GetDisplayNameOrStringValue**: Use this in most cases, especially in UI code where you always need a value to display. It's the safest option and won't throw exceptions.

2. **GetDisplayName**: Use this when you want to ensure all enum values have Display attributes and want to fail fast during development if one is missing.

3. **HasDisplayName**: Use this when you need to conditionally handle enum values differently based on whether they have Display attributes.

### Enum Definition Best Practices

```csharp
// ✅ Good: Consistent Display attributes
public enum PaymentMethod
{
    [Display(Name = "Credit Card")]
    CreditCard,
    
    [Display(Name = "PayPal")]
    PayPal,
    
    [Display(Name = "Bank Transfer")]
    BankTransfer
}

// ⚠️ Caution: Inconsistent attributes (use GetDisplayNameOrStringValue)
public enum NotificationPreference
{
    [Display(Name = "Email Notifications")]
    Email,
    
    Sms,  // No Display attribute
    
    [Display(Name = "Push Notifications")]
    Push
}
```

### Using with Localization

The Display attribute also supports resource files for localization:

```csharp
public enum Status
{
    [Display(Name = "ActiveStatus", ResourceType = typeof(Resources))]
    Active,
    
    [Display(Name = "InactiveStatus", ResourceType = typeof(Resources))]
    Inactive
}

// The GetDisplayName() method will automatically use the resource file
var display = Status.Active.GetDisplayName(); 
// Returns localized string from Resources.ActiveStatus
```

## Common Patterns

### Creating a Helper Method for Dropdowns

```csharp
public static class EnumHelper
{
    public static SelectList GetEnumSelectList<TEnum>() where TEnum : Enum
    {
        var items = Enum.GetValues<TEnum>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.GetDisplayNameOrStringValue()
            });
            
        return new SelectList(items, "Value", "Text");
    }
}

// Usage in controller:
ViewBag.StatusList = EnumHelper.GetEnumSelectList<UserStatus>();
```

### Extension Method for All Enum Values

```csharp
public static class EnumExtensionHelpers
{
    public static Dictionary<TEnum, string> GetAllDisplayNames<TEnum>() 
        where TEnum : Enum
    {
        return Enum.GetValues<TEnum>()
            .Cast<TEnum>()
            .ToDictionary(e => e, e => e.GetDisplayNameOrStringValue());
    }
}

// Usage:
var statusDisplayNames = EnumExtensionHelpers.GetAllDisplayNames<UserStatus>();
```

## Requirements

- Requires `System.ComponentModel.DataAnnotations` namespace for Display attribute
- Works with all .NET enum types
- Compatible with .NET Core 3.1+ and .NET 5+

## Related

- [System.ComponentModel.DataAnnotations.DisplayAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute)
- [Enum Class](https://docs.microsoft.com/en-us/dotnet/api/system.enum)
