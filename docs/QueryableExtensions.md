# QueryableExtensions

Extension methods for `IQueryable<T>` that provide conditional querying capabilities to simplify building dynamic queries.

## Namespace

```csharp
using ICG.NetCore.Utilities;
```

## Overview

The QueryableExtensions class provides a set of extension methods that allow you to conditionally apply LINQ operations to IQueryable sequences. This is particularly useful when building dynamic queries where certain filters or operations should only be applied based on runtime conditions.

## Methods

### WhereIf

Conditionally applies a filter to the query if the specified condition is true.

**Signature:**
```csharp
public static IQueryable<TSource> WhereIf<TSource>(
    this IQueryable<TSource> source,
    bool condition,
    Expression<Func<TSource, bool>> predicate)
```

**Parameters:**
- `source`: The source queryable sequence
- `condition`: If true, the predicate is applied; otherwise, the source is returned unchanged
- `predicate`: The filter expression to apply

**Returns:** The filtered queryable sequence if condition is true; otherwise, the original sequence

**Example:**
```csharp
var activeOnly = true;
var searchTerm = "John";

var users = dbContext.Users
    .WhereIf(activeOnly, u => u.IsActive)
    .WhereIf(!string.IsNullOrEmpty(searchTerm), u => u.Name.Contains(searchTerm))
    .ToList();

// This is cleaner than:
var query = dbContext.Users.AsQueryable();
if (activeOnly)
    query = query.Where(u => u.IsActive);
if (!string.IsNullOrEmpty(searchTerm))
    query = query.Where(u => u.Name.Contains(searchTerm));
var users = query.ToList();
```

### OrderByIf

Conditionally applies an ascending order to the query if the specified condition is true.

**Signature:**
```csharp
public static IQueryable<T> OrderByIf<T, TKey>(
    this IQueryable<T> query,
    bool condition,
    Expression<Func<T, TKey>> orderBy)
```

**Parameters:**
- `query`: The source queryable sequence
- `condition`: If true, the ordering is applied; otherwise, the source is returned unchanged
- `orderBy`: The key selector expression for ordering

**Returns:** The ordered queryable sequence if condition is true; otherwise, the original sequence

**Example:**
```csharp
var sortByName = true;

var users = dbContext.Users
    .WhereIf(activeOnly, u => u.IsActive)
    .OrderByIf(sortByName, u => u.LastName)
    .ToList();
```

### OrderByDescendingIf

Conditionally applies a descending order to the query if the specified condition is true.

**Signature:**
```csharp
public static IQueryable<T> OrderByDescendingIf<T, TKey>(
    this IQueryable<T> query,
    bool condition,
    Expression<Func<T, TKey>> orderBy)
```

**Parameters:**
- `query`: The source queryable sequence
- `condition`: If true, the ordering is applied; otherwise, the source is returned unchanged
- `orderBy`: The key selector expression for ordering

**Returns:** The ordered queryable sequence in descending order if condition is true; otherwise, the original sequence

**Example:**
```csharp
var sortDescending = true;

var users = dbContext.Users
    .OrderByDescendingIf(sortDescending, u => u.CreatedDate)
    .ToList();
```

### GetPage

Returns a specific page of results from the queryable sequence.

**Signature:**
```csharp
public static IQueryable<T> GetPage<T>(
    this IQueryable<T> query,
    int pageNumber,
    int pageSize)
```

**Parameters:**
- `query`: The source queryable sequence
- `pageNumber`: The page number to retrieve (1-based)
- `pageSize`: The number of items per page

**Returns:** A queryable sequence containing the specified page of results

**Example:**
```csharp
var pageNumber = 2;
var pageSize = 10;

var users = dbContext.Users
    .OrderBy(u => u.LastName)
    .GetPage(pageNumber, pageSize)
    .ToList();

// This retrieves items 11-20 (page 2 with 10 items per page)
```

**Important Note:** The query should be ordered before calling GetPage to ensure consistent pagination results.

### DistinctBy

Returns distinct elements from a sequence by using a specified key selector.

**Signature:**
```csharp
public static IQueryable<TSource> DistinctBy<TSource, TKey>(
    this IQueryable<TSource> query,
    Expression<Func<TSource, TKey>> keySelector)
```

**Parameters:**
- `query`: The source queryable sequence
- `keySelector`: A function to extract the key for each element

**Returns:** A queryable sequence that contains distinct elements based on the specified key

**Example:**
```csharp
// Get users with unique email addresses
var uniqueUsers = dbContext.Users
    .DistinctBy(u => u.Email)
    .ToList();

// Get orders with unique customer IDs
var uniqueCustomerOrders = dbContext.Orders
    .DistinctBy(o => o.CustomerId)
    .ToList();
```

## Real-World Usage Scenarios

### Building a Search API

```csharp
public class UserSearchService
{
    private readonly DbContext _context;

    public List<User> SearchUsers(UserSearchCriteria criteria)
    {
        return _context.Users
            .WhereIf(criteria.ActiveOnly, u => u.IsActive)
            .WhereIf(!string.IsNullOrEmpty(criteria.SearchTerm), 
                u => u.FirstName.Contains(criteria.SearchTerm) || 
                     u.LastName.Contains(criteria.SearchTerm))
            .WhereIf(criteria.MinAge.HasValue, u => u.Age >= criteria.MinAge.Value)
            .WhereIf(criteria.MaxAge.HasValue, u => u.Age <= criteria.MaxAge.Value)
            .OrderByIf(criteria.SortBy == "name", u => u.LastName)
            .OrderByDescendingIf(criteria.SortBy == "date", u => u.CreatedDate)
            .GetPage(criteria.PageNumber, criteria.PageSize)
            .ToList();
    }
}
```

### Filtering with Optional Parameters

```csharp
public IActionResult GetProducts(
    string category = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    bool inStock = false,
    int page = 1,
    int pageSize = 20)
{
    var products = _context.Products
        .WhereIf(!string.IsNullOrEmpty(category), p => p.Category == category)
        .WhereIf(minPrice.HasValue, p => p.Price >= minPrice.Value)
        .WhereIf(maxPrice.HasValue, p => p.Price <= maxPrice.Value)
        .WhereIf(inStock, p => p.StockQuantity > 0)
        .OrderBy(p => p.Name)
        .GetPage(page, pageSize)
        .ToList();
        
    return Ok(products);
}
```

## Benefits

1. **Cleaner Code**: Reduces the need for conditional if statements in query building
2. **Fluent Interface**: Maintains a fluent, chainable API style
3. **Better Readability**: Makes complex conditional queries more readable
4. **Type Safety**: Maintains full type safety with expression trees
5. **Query Optimization**: Conditions are evaluated before query execution, so only necessary predicates are sent to the database

## Notes

- All methods work with Entity Framework Core and any other IQueryable provider
- These extensions do not execute the query - they build the expression tree
- Remember to call `.ToList()`, `.ToArray()`, or similar to execute the query
- These are particularly useful in API endpoints where you need to build dynamic queries based on query string parameters
