# Technical architecture

This section covers the technical layout of the application and how to work with various parts
of the code. The whole api application is inside MinimalApi20-vsa.Api

## Working with use-cases

This project follows vertical slice architecture. Each use-case gets its own class under the Features
folder. The associated request and response objects are contained as subclass within the use-case
class. The use-case class has the name of the use case, for example:

- Creating a new product is implemented in Products/CreateProduct.cs
- Getting a product is implemented in Products/GetProduct.cs

- Each feature should be grouped into minimal api MapGroup with proper Openapi tags, and used extensions methods just like for products it is implemented in Products/ProductsGroup.cs


## Validating input to the use-cases

Each use-case request must be validated. We use [FluentValidation](https://docs.fluentvalidation.net/en/latest/).
You are only allowed the use the information in the request object to validate the contents.
If you need to validate something that involves a database then this is part of the logic in the use case.

## Handling API requests

The frontend is implemented as a single page application that talks to the minimal API endpoints. 

The whole request lifecycle happens inside the request handlers. This makes debugging and testing the code easier.

## Database

Database connection is made using EF Core. DbContext is defined in Persistance/AppDbContext.cs.
All entity models are stored inside Domain/Entities folder.

New entities should be created in that folder and added to AppDbContext.cs if needed. Any relations should be solved using proper relations convention on a model.

## Testing the components in the application

We require unit-tests written in XUnit in the `MinimalApi20-vsa.Tests` project.
The directory structure for the test project mirrors the structure of the agent project. This makes it easier
for us to find the test cases later.

We need tests that validate individual components and testss that combine every piece of the request lifecycle
in the request handlers of the application.

You can run tests with `dotnet test` from the root of the repository.

We use `Moq` for mocking dependencies in the application. You should use mocks for unit-tests to isolate
the unit under test. For integration tests you should only use mocks for dependencies that fall outside the application scope.