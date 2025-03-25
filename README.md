


# Armut Project Explanation

# Ratings & Notifications Microservices Documentation

## Overview

Two interconnected microservices:
- **Ratings API**: Handles rating operations (create, get average)
- **Notifications API**: Processes rating events and manages notifications

## Key Features

### Architecture
- **Clean Architecture** pattern
- Controller → Service → Repository separation
- DTOs for data transfer
- Entity models for database

### Messaging
- **MassTransit** with RabbitMQ
- Event-driven communication
- Message retry mechanisms
- Concurrency handling

### Validation & Error Handling
- FluentValidation integration
- Custom exception types:
  - `BusinessException`
  - `GlobalException`
- Global exception middleware

### API Features
- Versioning support
- Swagger/OpenAPI docs
- In-memory databases for development
- Structured logging

## Technology Stack

| Component          | Technology       |
|--------------------|------------------|
| Framework          | .NET 8           |
| ORM                | EF Core          |
| Messaging          | MassTransit      |
| Message Broker     | RabbitMQ         |
| Validation         | FluentValidation |
| API Documentation  | Swagger          |
| Containerization   | Docker           |

## Advantages

✔ **Loose Coupling**  
Services communicate via events, not direct calls

✔ **Reliability**  
Built-in retry mechanisms and concurrency handling

✔ **Maintainability**  
Clear separation of concerns and consistent patterns

✔ **Extensibility**  
Easy to add new consumers and extend validation

## Getting Started

### Prerequisites
- Docker
- .NET 8 SDK

# Setup on Docker

## Network
###  Use for connecting each other to same network on docker so they can communicate
###  Create a Docker network
docker network create my-armut-network  

###  Connect RabbitMQ container to the network
docker run --name rabbit-test --network my-armut-network -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest -p 5672:5672 -p 15672:15672 rabbitmq:3.10.2-management

## Images And Containers

###  Create Image for Notification
docker build -t notifications -f Notifications.Api/Dockerfile . 
###  Create Image for Ratings
docker build -t ratings -f Ratings.Api/Dockerfile .


###  Create and connect notifications application container to the network
docker run --name notifications --network my-armut-network -e ASPNETCORE_ENVIRONMENT=Development -p 8082:8080 -p 8083:8081 notifications
###  Create and connect ratings application container to the network
docker run --name ratings --network my-armut-network -e ASPNETCORE_ENVIRONMENT=Development -p 8084:8080 -p 8085:8081 ratings


## Single Usages
### Create Rabbitmq
docker run -d --name rabbit-test -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest -p 5672:5672 -p 15672:15672 rabbitmq:3.10.2-management
### Create Container for Notification
docker run --name notifications -e ASPNETCORE_ENVIRONMENT=Development -p 8082:8080 -p 8083:8081 notifications
### Create Container for Ratings
docker run --name ratings -e ASPNETCORE_ENVIRONMENT=Development -p 8084:8080 -p 8085:8081 ratings