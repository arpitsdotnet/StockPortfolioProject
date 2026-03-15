# Implementation Plan Template

Use this template when planning new features or refactoring existing code. **Complete this plan before writing any code.**

---

## ?? Feature Overview

**Feature Name:** [Enter feature name]  
**Description:** [Brief description of what the feature does]  
**Related User Story/Issue:** [Link or reference]  
**Estimated Complexity:** Low / Medium / High  

---

## ?? Objectives

- [ ] Objective 1
- [ ] Objective 2
- [ ] Objective 3

---

## ?? Design Overview

### Architecture Pattern
- **Pattern:** Handler Pattern / Service Pattern / Repository Pattern
- **Reasoning:** [Explain why this pattern is chosen]

### Data Flow
```
[Entry Point] ? [Handler/Service] ? [DbContext] ? [Database]
     ?              ?
  Controller    Validation, Business Logic
```

---

## ?? Files to Create

### New Classes/Interfaces
| File Path | Type | Purpose |
|-----------|------|---------|
| `StockPortfolio.Core/Features/[Feature]/[Action]/[ActionRequest].cs` | Record/Class | Request DTO |
| `StockPortfolio.Core/Features/[Feature]/[Action]/[ActionResponse].cs` | Record/Class | Response DTO |
| `StockPortfolio.Core/Features/[Feature]/[Action]/[ActionHandler].cs` | Class | Business logic handler |
| `StockPortfolio.API/Controllers/[Feature]Controller.cs` | Class | HTTP endpoints |

### Example for Share Price Creation:
| File Path | Type | Purpose |
|-----------|------|---------|
| `StockPortfolio.Core/Features/SharePrices/CreateSharePrice/CreateSharePriceRequest.cs` | Record | Request DTO |
| `StockPortfolio.Core/Features/SharePrices/CreateSharePrice/CreateSharePriceResponse.cs` | Record | Response DTO |
| `StockPortfolio.Core/Features/SharePrices/CreateSharePrice/CreateSharePriceHandler.cs` | Class | Handler |

---

## ?? Files to Modify

| File Path | Changes | Reason |
|-----------|---------|--------|
| `StockPortfolio.API/Program.cs` | Register handler in DI container | Enable dependency injection |
| `StockPortfolio.API/Controllers/[Feature]Controller.cs` | Add HTTP endpoints | Expose functionality via API |
| `[Other files]` | [Describe changes] | [Explain reason] |

### Example:
| File Path | Changes | Reason |
|-----------|---------|--------|
| `Program.cs` | `services.AddScoped<CreateSharePriceHandler>();` | Register handler |
| `SharePricesController.cs` | Add `[HttpPost]` method | Expose Create endpoint |

---

## ?? Dependencies & Integration

### New Dependencies
- [ ] `ApplicationDbContext` (DbContext)
- [ ] `ILogger<T>` (Logging)
- [ ] [External services/APIs]

### Database Changes
- [ ] No database changes needed
- [ ] Modify existing table: [Table name]
- [ ] Create new table: [Table name]
- [ ] Create EF Core migration: `Add-Migration [MigrationName]`

### External Integrations
- [ ] None
- [ ] Alpha Vantage API
- [ ] [Other API/Service]

---

## ?? Input Validation

### Request Validation Rules
```csharp
public class CreateSharePriceRequest
{
    // Validation rules
    // Example: SecurityId must exist
    // Example: Price must be > 0
    // Example: Date cannot be in future
}
```

**Validation Logic Location:** Handler constructor / Handle method  
**Error Codes:** [List applicable error codes]

---

## ?? Business Logic Steps

### Core Algorithm/Process
1. **Validation:** Validate input constraints
   - [ ] Check request is not null
   - [ ] Validate data types and ranges
   - [ ] Verify related entities exist

2. **Processing:** Execute business logic
   - [ ] Query/fetch required data from database
   - [ ] Perform calculations or transformations
   - [ ] Apply business rules

3. **Persistence:** Save changes
   - [ ] Create/update entity
   - [ ] Add to DbContext
   - [ ] Call `SaveChangesAsync()`

4. **Response:** Return result
   - [ ] Map entity to DTO
   - [ ] Return `Result<T>.Success(dto)` or `Result<T>.Failure(error)`

### Example: Create Share Price
```csharp
1. Validate request (price > 0, date valid, security exists)
2. Check if security ID exists in database
3. Create new SharePrice entity
4. Add to _context.SharePrices
5. Save to database
6. Map to response DTO
7. Return Result<CreateSharePriceResponse>.Success()
```

---

## ?? Error Handling Strategy

### Expected Error Scenarios

| Scenario | Error Type | Error Code | HTTP Status | Message |
|----------|-----------|-----------|------------|---------|
| Request is null | VALIDATION | BAD_REQUEST | 400 | "Request body is required" |
| Security doesn't exist | VALIDATION | NOT_FOUND | 404 | "Security not found" |
| Invalid price (?0) | VALIDATION | BAD_REQUEST | 400 | "Price must be greater than zero" |
| Duplicate share price | BUSINESS | CONFLICT | 409 | "Share price for this date already exists" |
| Database error | SYSTEM | INTERNAL_ERROR | 500 | "An error occurred processing your request" |

### Error Mapping in Controller
```csharp
if (result.IsFailure)
{
    return result.Error.Code switch
    {
        ErrorCode.BAD_REQUEST => BadRequest(...),
        ErrorCode.NOT_FOUND => NotFound(...),
        ErrorCode.CONFLICT => Conflict(...),
        _ => StatusCode(500, ...)
    };
}
```

---

## ?? Testing Strategy

### Unit Tests
- [ ] Handler validation tests
- [ ] Handler business logic tests
- [ ] Handler error handling tests
- [ ] Controller response mapping tests

### Integration Tests
- [ ] End-to-end API tests
- [ ] Database persistence tests

### Test File Locations
```
StockPortfolio.Core.UnitTests/
??? Features/[Feature]/
    ??? [Action]HandlerTests.cs
    ??? [Action]ValidatorTests.cs

StockPortfolio.API.IntegrationTests/
??? Controllers/
    ??? [Feature]ControllerTests.cs
```

### Sample Test Cases
```csharp
// CreateSharePriceHandlerTests.cs

[Fact]
public async Task Handle_WithValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new CreateSharePriceRequest(...);
    
    // Act
    var result = await _handler.Handle(request, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}

[Fact]
public async Task Handle_WithNullRequest_ReturnsFailure()
{
    // Arrange
    CreateSharePriceRequest request = null;
    
    // Act
    var result = await _handler.Handle(request, CancellationToken.None);
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(ErrorCode.BAD_REQUEST, result.Error.Code);
}
```

---

## ?? Implementation Checklist

### Code Structure
- [ ] Request/Response DTOs created
- [ ] Handler class created with `Handle()` method
- [ ] XML documentation added (`///` comments)
- [ ] 1-2 liner comments for complex logic
- [ ] Error handling implemented
- [ ] Input validation implemented
- [ ] One class per file (naming matches filename)

### API Integration
- [ ] Controller endpoints created
- [ ] HTTP methods and routes correct
- [ ] HTTP status codes appropriate
- [ ] Response wrapping in `ResultDto<T>`
- [ ] Error responses mapped to correct HTTP codes

### Dependency Injection
- [ ] Handler registered in `Program.cs`
- [ ] Correct lifetime scope (`Scoped`, `Transient`)
- [ ] Constructor parameters properly injected
- [ ] No circular dependencies

### Database
- [ ] No new tables required OR migrations created
- [ ] DbContext queries optimized (`AsNoTracking()` where applicable)
- [ ] N+1 query issues prevented (`Include()` used)
- [ ] `CancellationToken` passed to async DB calls

### Documentation
- [ ] XML docs on all public members
- [ ] Complex logic commented (1-2 liners)
- [ ] README updated if needed
- [ ] API documentation updated

### Testing
- [ ] Unit tests written (minimum 3 test cases)
- [ ] Integration tests written
- [ ] All tests passing
- [ ] Code coverage > 80%

### Code Review Readiness
- [ ] Code compiles without warnings
- [ ] Follows naming conventions
- [ ] Follows SOLID principles
- [ ] No hardcoded values (use constants)
- [ ] No commented-out code

---

## ?? Implementation Order

1. **Phase 1 - Setup**
   - [ ] Create request/response DTOs
   - [ ] Register dependencies in `Program.cs`

2. **Phase 2 - Core Logic**
   - [ ] Implement handler with validation
   - [ ] Implement business logic
   - [ ] Add error handling

3. **Phase 3 - API Integration**
   - [ ] Create controller
   - [ ] Add HTTP endpoints
   - [ ] Map errors to HTTP status codes

4. **Phase 4 - Testing**
   - [ ] Write unit tests
   - [ ] Write integration tests
   - [ ] Verify all tests pass

5. **Phase 5 - Documentation**
   - [ ] Add XML docs
   - [ ] Add inline comments
   - [ ] Update README/API docs

---

## ?? Key Standards to Follow

? **Must Follow:**
- Handler pattern for business logic (no direct controller logic)
- `Result<T>` return type for operations that can fail
- Constructor-based DI only
- One class per file
- Async/await for all I/O
- Always pass `CancellationToken`
- Use DbContext directly (no repository pattern)

? **Must Avoid:**
- Hardcoded values (use constants or configuration)
- Synchronous database calls
- Null references (use `??` or `?.` operators)
- Exposing internal exceptions to clients
- Skipping input validation
- N+1 query problems

---

## ?? Related Documentation

- See `.copilot-instructions.md` for project standards
- See `/StockPortfolio.Core/Features/SharePrices/` for example implementation
- See `README.md` for project overview

---

## ? Sign-Off

**Plan Reviewed:** [ ] Yes [ ] No  
**Reviewer:** ________________  
**Date:** ________________  
**Comments/Concerns:**
```
[Add any notes or concerns here]
```

**Ready for Implementation:** [ ] Yes [ ] No (if No, list blockers)

---

## ?? Notes & Additional Context

```
[Add any additional context, decisions, or notes about this feature]
```

---

**Created on:** [Date]  
**Last Updated:** [Date]  
**Status:** Planning / In Progress / Completed / On Hold
