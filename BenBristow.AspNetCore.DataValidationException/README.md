# BenBristow.AspNetCore.DataValidationException

[![NuGet](https://img.shields.io/nuget/v/BenBristow.AspNetCore.DataValidationException.svg)](https://www.nuget.org/packages/BenBristow.AspNetCore.DataValidationException/)

A library for handling data validation exceptions in ASP.NET Core, with support for mapping domain properties to model properties.

## Installation

```bash
dotnet add package BenBristow.AspNetCore.DataValidationException
```

## Features

- **DataValidationException**: A custom exception for capturing validation errors with property-level granularity
- **DataValidationExceptionFactory**: A fluent builder for constructing validation exceptions
- **MapsToPropertyAttribute**: Map domain property names to model property names for seamless error mapping
- **ModelState Integration**: Automatically add validation errors to ASP.NET Core ModelState

## Usage

### Basic Usage

```csharp
using BenBristow.AspNetCore.DataValidationException.Exceptions;

// Create a validation exception with a single error
throw new DataValidationException("Invalid data");

// Create a validation exception with multiple errors
var errors = new Dictionary<string, IEnumerable<string>>
{
    { "Email", new[] { "Email is required", "Email must be valid" } },
    { "Password", new[] { "Password must be at least 8 characters" } }
};
throw new DataValidationException(errors);
```

### Using the Factory Pattern

```csharp
using BenBristow.AspNetCore.DataValidationException.Exceptions;

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}

// Build and throw validation exception using the factory
var factory = new DataValidationExceptionFactory<UserRegistrationModel>();

factory
    .AddError(m => m.Email, "Email is required")
    .AddError(m => m.Email, "Email must be valid")
    .AddError(m => m.Password, "Password must be at least 8 characters")
    .AddError("General error not tied to a specific field");

throw factory.Create();
```

### Mapping Domain Properties to Model Properties

When your domain model uses different property names than your API models, use the `MapsToPropertyAttribute` to ensure errors are mapped correctly:

```csharp
using BenBristow.AspNetCore.DataValidationException.Attributes;

public class UserApiModel
{
    [MapsToProperty("EmailAddress")]  // Maps to domain property "EmailAddress"
    public string Email { get; set; }
    
    [MapsToProperty("UserPassword")]  // Maps to domain property "UserPassword"
    public string Password { get; set; }
}
```

### Adding Errors to ModelState

In your ASP.NET Core controller, catch the exception and add errors to ModelState:

```csharp
using Microsoft.AspNetCore.Mvc;
using BenBristow.AspNetCore.DataValidationException.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] UserApiModel model)
    {
        try
        {
            // Your business logic that might throw DataValidationException
            _userService.CreateUser(model);
            return Ok();
        }
        catch (DataValidationException ex)
        {
            // Add validation errors to ModelState with property mapping
            ex.AddToModelState<UserApiModel>(ModelState);
            return BadRequest(ModelState);
        }
    }
}
```

The `AddToModelState<T>` method will automatically:
1. Map domain property names to model property names using `MapsToPropertyAttribute`
2. Add all validation errors to the ModelState
3. Preserve property names for unmapped properties

### Complete Example

```csharp
// Domain Service
public class UserService
{
    public void CreateUser(UserApiModel model)
    {
        var factory = new DataValidationExceptionFactory<UserApiModel>();
        
        if (string.IsNullOrEmpty(model.Email))
            factory.AddError(m => m.Email, "Email is required");
            
        if (model.Password?.Length < 8)
            factory.AddError(m => m.Password, "Password must be at least 8 characters");
            
        if (factory.HasErrors)
            throw factory.Create();
            
        // Continue with user creation...
    }
}

// API Model
public class UserApiModel
{
    [MapsToProperty("EmailAddress")]
    public string Email { get; set; }
    
    [MapsToProperty("UserPassword")]
    public string Password { get; set; }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    
    public UsersController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public IActionResult CreateUser([FromBody] UserApiModel model)
    {
        try
        {
            _userService.CreateUser(model);
            return Ok();
        }
        catch (DataValidationException ex)
        {
            ex.AddToModelState<UserApiModel>(ModelState);
            return BadRequest(ModelState);
        }
    }
}
```