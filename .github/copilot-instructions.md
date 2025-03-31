# Technical architecture

This section covers the technical layout of the application and how to work with various parts
of the code.

## Working with use-cases

This project follows vertical slice architecture. Each use-case gets its own class under the feature
folder. The associated request and response objects are contained as subclass within the use-case
class. The use-case class has the name of the use case, for example:

- Creating a new conversation is implemented in Conversations/CreateConversation.cs
- Sending a message is implemented in Conversations/SendMessage.cs
- Uploading attachments is implemented in Attachments/UploadAttachment.cs



## Validating input to the use-cases

Each use-case request must be validated. We use [FluentValidation](https://docs.fluentvalidation.net/en/latest/).
You are only allowed the use the information in the request object to validate the contents.
If you need to validate something that involves a database then this is part of the logic in the use case.

## Handling API requests

The frontend is implemented as a single page application that talks to the minimal API endpoints. 

The whole request lifecycle happens inside the request handlers. This makes debugging and testing the code easier.

## Testing the components in the application

We require unit-tests written in XUnit in the `MinimalApi20.Tests` project.
The directory structure for the test project mirrors the structure of the agent project. This makes it easier
for us to find the test cases later.

We need tests that validate individual components and testss that combine every piece of the request lifecycle
in the request handlers of the application.

You can run tests with `dotnet test` from the root of the repository.

We use `Moq` for mocking dependencies in the application. You should use mocks for unit-tests to isolate
the unit under test. For integration tests you should only use mocks for dependencies that fall outside the application scope.