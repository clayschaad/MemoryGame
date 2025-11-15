# MemoryGame
MemoryGame is a Blazor WebAssembly application for people with dementia

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## General rules

- Use dotnet version 10
- Always build and run test
- Update README.md automatically if a code change need to be addressed

## C# Coding Standards
- Should use params collections (e.g., ```csharp
void Foo(params IReadOnlyList<string> values) => // actual implementation here.```)
- Using Lock instead of new object() makes the intent of the code clear and there also might be performance benefits due to special casing of the new type in the .NET runtime.
- Should use Primary constructors
- Should use collection expressions & spread operator
- Don't use ref readonly parameters

### Code Quality
- Write unit tests for new features and bug fixes whenever possible and it provides value
- Treat warnings as errors in builds to prevent minor issues from accumulating (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`)

### Namespace Guidelines
- Use consistent namespace structure following folder hierarchy
- Avoid deep nesting - keep namespaces reasonable length
- Group related functionality in same namespace

### API Controllers
- Use proper route attributes with versioning (e.g., `api/v1/`, `api/v2/`)
- Follow kebab-case for URLs where required, suppress warnings where legacy URLs exist
- Always include proper HTTP status code responses with SwaggerResponse attributes
- Validate input parameters and return appropriate error responses

### REST API Design
- Use HTTP verbs correctly (GET for queries, POST for commands, PUT for updates, DELETE for removal)
- Return appropriate HTTP status codes (200, 201, 400, 404, 409, 500)
- Use consistent error response format
- Support pagination for collection endpoints
- Include resource identifiers in URLs: `/api/v1/shifts/{shiftId}`
- Use plural nouns for resource collections: `/api/v1/shifts`

### Command API Design
- Commands are named imperative: `CreateShift`, `UpdateShift`
- Commands have no Pre-/Suffix "Command"

### Async Guidelines
- Always use `async`/`await` for I/O operations
- Include `CancellationToken` as last parameter in async methods
- Don't block on async code with `.Result` or `.Wait()` - if needed, use `SynchronousWaiter.WaitFor()`
- Name async methods with `Async` suffix

### Collections Guidelines
- Do not use immutable collections and try to remove them from the codebase.
- Use `IReadOnlySet<T>` and `IReadOnlyDictionary<T>` backed by mutable `HashSet<T>` and `Dictionary<T>` for very short lived collections (like most business logic)
- Prefer `List<T>` over `IList<T>` for mutable collections when concrete type is needed
- Use `HashSet<T>` for uniqueness, `List<T>` for ordering
- Use `FrozenSet<T>, FrozenDictionary<T>, ImmutableArray<T>` for long living data which is read often (e.g. Caches)
- Initialize collections at declaration when possible
- Avoid returning null collections - return empty collections instead

### Nullable Handling
- Use nullable reference types and try to remove #nullable disable from the codebase
- Use proper null checks and null-coalescing operators
- Use null-conditional operators where appropriate: `obj?.Property`

### Dependency Injection
- Use constructor dependency injection
- Register services with appropriate lifetime (Singleton, Scoped, Transient)
- Avoid service locator pattern

### API Documentation
- Document public APIs
- Include proper Swagger annotations
- Provide examples in documentation whenever possible
- Keep documentation up to date with code changes

### Error Handling
- Include meaningful error messages
- Log exceptions with appropriate severity levels
- Don't expose internal implementation details in error messages

### File Organization
- Use file-scoped namespaces
- Keep one class per file

### Performance
- Avoid unnecessary allocations
- Avoid calling methods that access APIs or databases inside loops

### Documentation
- Update the README.md file to reflect important changes

## Domain-Specific Patterns

### ID Types and Records
- Replace complex Id types with primitive types wherever possible
- Use readonly record structs for immutable data

### Service Interfaces
- Add interfaces to services only when absolutely necessary. Try to remove interfaces with a single implementation.
- Prefix service interfaces with `I`: `IMyService`
- Use specific return types rather than generic objects

### Equality Implementation
- Override `Equals` and `GetHashCode` consistently
- Implement `IEquatable<T>` for value types and data objects - but not for records
- Use proper null checks in equality methods
- Consider `ReferenceEquals` optimization for reference types

### Date, Time and TimeZone
- Don't use DateTime for a date with time, use DateTimeOffset instead
- When only a date is needed, use DateOnly
- When only a time is needed, use TimeOnly
- Always use a specific local timezone for the DateTimeOffset
- If nothing specified, use Europe/Zurich

### Architecture
- Proper separation of concerns
- Following established patterns in the codebase
- Do not use any sort of "aggregate root" concept, instead use simple entities and value objects.

### Testing
- Ensure testability through proper dependency injection
- Mock external dependencies
- Test both success and error scenarios

### Common Instructions
- Don't write code comments
- Use var instead of explicit type, ex. var x = 4;
