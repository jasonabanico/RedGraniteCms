# RedGraniteCms: React + Redux Toolkit + GraphQL + .Net Boilerplate

## Description
RedGraniteCms is a simple CMS implementation built on RedGranite, a full-stack boilerplate designed to help developers quickly build applications using React, Redux Toolkit, GraphQL, and .Net. It provides a solid foundation for creating scalable and maintainable applications with full CRUD (Create, Read, Update, Delete) functionality, leveraging GraphQL for efficient data operations.

## Features

### Server-Side (.NET 9.0 / HotChocolate)
- **Clean Architecture** - Core → Data → Services → GraphQL → API layers
- **Rich Domain Models** - Factory methods with encapsulated validation
- **Repository Pattern** - Interface-based dependency injection
- **Structured Logging** - `ILogger<T>` throughout all layers
- **Input Validation** - FluentValidation for GraphQL inputs
- **Error Handling** - Custom exceptions with HotChocolate `IErrorFilter`
- **Authorization** - Azure AD integration with `[Authorize]` attributes
- **Unit Tests** - xUnit with FluentAssertions and Moq

### Client-Side (React 18 / Redux Toolkit / Apollo)
- **Vite Build System** - Fast development with TypeScript 5.x
- **Feature-Based Structure** - Co-located components, slices, and selectors
- **Type-Safe GraphQL** - Apollo codegen for TypeScript types
- **Form Validation** - Zod schemas with React Bootstrap integration
- **Error Boundary** - Graceful crash recovery
- **Loading States** - Spinner and error components
- **Environment Config** - `.env` files for API endpoints
- **Auth-Ready Apollo Client** - Link chain with token injection

## Quick Start

```bash
# Clone the repository
git clone https://github.com/jasonabanico/RedGraniteCms.git

# Server
cd src
dotnet restore
dotnet build

# Client
cd src/Client/RedGraniteCms.Client.Web
npm install
npm run dev
```

## Environment Variables

### Client
Create `.env.development` and `.env.production` in the client folder:

```env
VITE_GRAPHQL_URL=http://localhost:5034/graphql
```

### Server
Configure in `appsettings.json`:
- `AzureAd` - Azure AD authentication settings
- `Cors:AllowedOrigins` - Allowed CORS origins
- `ConnectionStrings:CosmosConnection` - CosmosDB connection string

## Docs
- [Setup](https://github.com/jasonabanico/RedGraniteCms/blob/main/docs/setup.md)
- [GraphQL](https://github.com/jasonabanico/RedGraniteCms/blob/main/docs/graphql.md)
- [Extending and Customizing](https://github.com/jasonabanico/RedGraniteCms/blob/main/docs/customizing.md)

## Project Structure

```
src/
├── Server/
│   ├── RedGraniteCms.Server.Api/          # ASP.NET Core API host
│   ├── RedGraniteCms.Server.Core/         # Domain models & interfaces
│   │   ├── Exceptions/                 # Custom exception types
│   │   ├── Interfaces/                 # Repository & service contracts
│   │   └── Models/                     # Domain entities
│   ├── RedGraniteCms.Server.Data/         # EF Core + CosmosDB
│   ├── RedGraniteCms.Server.Services/     # Business logic layer
│   ├── RedGraniteCms.Server.GraphQl/      # HotChocolate GraphQL
│   │   ├── Errors/                     # Error filter
│   │   ├── Mutations/                  # GraphQL mutations
│   │   ├── Queries/                    # GraphQL queries
│   │   ├── Types/                      # Input types
│   │   └── Validators/                 # FluentValidation validators
│   ├── RedGraniteCms.Server.Core.Tests/   # Domain model tests
│   └── RedGraniteCms.Server.Services.Tests/ # Service layer tests
└── Client/
    └── RedGraniteCms.Client.Web/
        └── src/
            ├── app/                    # Redux store & typed hooks
            ├── components/             # Shared UI components
            ├── features/               # Feature modules
            ├── graphql/                # Apollo client setup
            ├── pages/                  # Page components
            └── services/               # GraphQL service layer
```

## Running Tests

```bash
# Server
cd src
dotnet test

# Client
cd src/Client/RedGraniteCms.Client.Web
npm test
```

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## References
- [React](https://reactjs.org/)
- [Redux Toolkit](https://redux-toolkit.js.org/)
- [GraphQL](https://graphql.org/)
- [Apollo GraphQL Client](https://www.apollographql.com/docs/react/development-testing/static-typing)
- [HotChocolate GraphQL Server](https://chillicream.com/docs/hotchocolate/v13)
- [.Net](https://docs.microsoft.com/en-us/dotnet/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Zod](https://zod.dev/)

## Additional Notes
- This boilerplate is designed for scalability and includes best practices for state management, data fetching, and backend development.
- For any issues or feature requests, please open an issue on the GitHub repository.
