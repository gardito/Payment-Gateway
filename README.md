# Installation

## Prerequisites

The project runs on .NET Core SDK 7.

It is recommended to use an IDE to explore the project (Visual Studio or Rider).

## Run the application

### Command Line

From the root folder of the project, change directory to the PaymentGateway.WebApi project. Run the project by using `dotnet` command and open Swagger webpage to test the APIs.

- Run `cd PaymentGateway.WebApi`
- Run `dotnet run`
- open http://localhost:5001/swagger/index.html

### IDE

The project was built using JetBrains Rider and the commands can change from IDE to IDE.

To run the project from the IDE, simply run the project using the run button. Be sure to run the PaymentGateway.WebApi. Both `Debug` or `Release` are fine.

Usually the IDE should launch Swagger in the web browser. If not, use the following link: http://localhost:5001/swagger/index.html

# Project Overview

The project was built following both the Clean Architecture and Domain-Driven Design.

The main idea is to define the Domain at the core of the architecture with no external dependencies.

Next to the Domain layer, there is the Service layer. This is the layer where information is sent or fetched. The Service layer has abstract dependencies to repositories and third-party services, like databases or external libraries.

Next, there is the Infrastructure layer. Here is where the technical decisions are implemented in concrete technologies. For example: databases, cache, web APIs.

## Common

The `Common` class library contains a `Result` classed used as a return value, together with the `Error` class and `ErrorCode` enum.

## Domain

The Domain layer is defined in the `Domain` solution folder and there are 4 class libraries.

### Domain.Common

The `Domain.Common` class library contains classes used in all the domain class libraries. At the moment, it contains the Error definitions for the domain validation and it is referenced by the `Payment.Domain` class library.

### Merchant.Domain

The `Merchant.Domain` class library contains the basic definition for the Merchant domain object.

### Payment.Domain

The `Payment.Domain` class library contains the domain logic for payments. Here, the `Payment.cs` class defines all the components to represents a payment (including the Value Objects), the Service interface definition, and the Repository interface definition.

### Payment.Domain.UnitTests

The `Payment.Domain.UnitTests` class library contains the unit tests for the logic implemented in the `Payment.Domain` class library.

## Services

The Service layer is defined in the `Services` solution folder and there are 3 class libraries.

### Payment.Service

The `Payment.Service` class library is the actual implementation of the Payment service interface defined in `Payment.Domain`.

The `PaymentService` class interacts with the payment repository and the third-party banking service.

### Payment.Service.UnitTests

The `Payment.Service.UnitTests` class library contains the unit tests for the logic implemented in the `Payment.Service` class library.

### Service.Common

The `Service.Common` class library contains the `BankPaymentResult` class to represent an error in the interaction with the third-party banking service.

## Infrastructure

The Infrastructure layer is defined in the `Infrastructure` solution folder and there are 7 class libraris. Here is where all the technical abstractions are implemented in specific technologies.

### Cache.Common

The `Cache.Common` class library contains the interface definition for the idempotency checker for payments.

### Cache.InMemory

The `Cache.InMemory` class library contains a simple in-memory caching mechanism for idempotency verifications for incoming payments. It is the implementation for the `IIdempotencyCachingSystem` defined in `Cache.Common`.

### Data.Payment.InMemoryDb

The `Data.Payment.InMemoryDb` class library is the actual implementation of the Payment repository interface defined in `Payment.Domain`. It defines a simple in-memory database to store the payments.

### Data.Payment.InMemoryDb.IntegrationTest

The `Data.Payment.InMemoryDb.IntegrationTest` class library contains the integration tests for the database implementation. It tests the correctness of the database operations.

### Database.Common

The `Database.Common` class library contains the database-related errors.

### PaymentGateway.WebApi

The `PaymentGateway.WebApi` class library contains the Web API project that accepts incoming requests from the clients. It is defined the Payment controller responsible for payments management and contains the `HTTP Get - GetPaymentById` and the `HTTP Post - ProcessPayment` endpoints.

### PaymentGateway.WebApi.UnitTests

The `PaymentGateway.WebApi.UnitTests` class library contains the unit tests for the controllers.

## Third-Party Services

The third-party services solution folder contains the interface definition of an external banking service (`Service.Banking.Interfaces` class library) and the implmentation of the CKO Bank Simulator (`Service.Banking.CkoBankSimulator` class library).

## Dependency Injection and Inversion of Control

Dependency Injection is used in the project to inject abstractions into classes like services or controllers. This make it easier to depend on definitions rather than implementations. Moreover, Dependency Injections helps automated testing too by using mocked implementations.

The Inversion of Control container is defined at API level.

The Payment service and the Third-Party Banking service are defined with `Scoped` lifetime to free up the resources once a request is completed.

The Idempotency Caching System is defined as `Singleton` so that always the same instance is returned every time an idempotency validation takes place.

The Payment Repository is defined as `Singleton` too. This is because of the nature of the project and for the database. The in-memory database will lose its data every time a request is completed, hence why it was defined as singleton. Usually a repository instance should not be defined as Singleton as we don't want to keep a database connection open all the time.

# API Documentation

There are 2 Restful API endpoints, `GetPaymentById` and `ProcessPayment`.

## [GET] /api/Payments/{guid}

Returns the Payment details given a GUID.

### Responses

The endpoint returns 200 on an existing payment record, 404 on a payment not found and a 400 on invalid input or handled lower level exceptions.

Successful Request Example:

```
{
  "id": "23a9663c-37af-4f45-a72e-82114d9e718c",
  "transactionDate": "07/09/2023 09:39:06",
  "merchantId": 1,
  "merchantName": "Merchant Name",
  "amount": 100,
  "currency": "EUR",
  "cardNumber": "***********1234",
  "status": "Approved"
}
```

## [POST] /api/Payments

Process the incoming payment requests given the following incoming body payload definition:

```
{
  "idempotencyId": "3fa85f64-5717-4562-b3fc-2c963f66afb7",
  "merchantId": 1,
  "merchantName": "Merchant Name",
  "amount": 50.5,
  "currency": "GBP",
  "card": {
    "ownerFullName": "Giulio Ardito",
    "cardNumber": "4434 1234 1234 1234",
    "cvv": "123",
    "expiryMonth": "07",
    "expiryYear": "2023"
  }
}
```

The body payload contains and `idempotencyId` field to distinguish processed payments for unprocessed ones.

### Responses

The endpoint returns 200 on successful payment process and 400 on payments declined, already processed payments or invalid input parameters.

Successful Request Example:

```
{
  "paymentId": "f1eb9a65-66f9-4ef2-86cb-550f859dfa8e"
}
```

Unsuccessful Request Example:

```
{
  "paymentId": "cc8ee355-7c14-4e1d-b773-01709711a372",
  "errorMessage": "Payment declined"
}
```

# What's next

## Improvements

- Centralized logging system.
- Authorization and Authentication system to access sensible endpoints
- ApiKey for merchants
- End-to-end tests
