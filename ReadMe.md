# Dating App Using .NET Microservices

<hr>

## Overview

Building a African origin Dating application for African diaspora. A user will meet partner from the same country origin.
User will be able to create a profile and browse the list of opposite sex of the same country of origin.
User will also have the ability to chat with the matched partner.

## Architecture

### Services

- API Gateway
- Auth Service - responsible of authenticating the user and using JWT Tokens.
- Profile Service
- Admin Service
- Chat Service - Use SignalR for one to one chat when match
- Matched Service

## Environment

this is cross platform application because we implementing the .NET Core

### technologies stack

- .NET Core 5
- JWT tokens
- Entity Framework Core
- Ocelot
- RabbitMQ
- MediatR and CQRS
