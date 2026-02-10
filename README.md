# Portfolio
- Made entirely in C# .NET framework
- Splitting different ways of working into different modules:
    - Pathfinding
    - Hospital (appointment management with automatic invoices, optimized searches & management of patients and doctors)
    - DocuGroup (Working with multiple people on the same document with real time changes) (WIP!!!)
    
## Tech Stack
- C# .NET Framework
- Entity Framework
- LINQ
- PostgreSQL
- Redis
- Keycloak (for authentication and authorization)

## How to run
- run `dotnet restore` to install dependencies
- run `dotnet build` to build the application
- run `docker compose up` from /DAL/Docker to start the PostgreSQL, Redis and Keycloak containers
- run `dotnet run` to start the application
- application will be available at `http://localhost:5266`
- Keycloak will be available at `http://localhost:8080` (username: admin, password: admin)
- to run tests, navigate to the test project and run `dotnet test`


