# RubroX

This repository contains the RubroX solution, a multi-project .NET application with accompanying frontend and identity services. The workspace is structured as follows:

- **aspire/**: Contains `RubroX.AppHost` and service defaults used by the ASP.NET host.
- **frontend/**: Angular-based UI located in `rubrox-ui` with build configuration and Docker support.
- **identity/**: Identity server project (`RubroX.Identity`) handling authentication and user management.
- **infra/**: Infrastructure artifacts, including database initialization SQL scripts.
- **src/**: Core solution projects:
  - `RubroX.API`: Web API project exposing endpoints.
  - `RubroX.Application`: Application layer with commands, queries, and DTOs.
  - `RubroX.Domain`: Domain layer containing aggregates, value objects, services.
  - `RubroX.Infrastructure`: Technical infrastructure including persistence and services.
- **tests/**: Unit and integration test projects for each layer plus E2E tests.

## Building & Running

```bash
# restore and build the full solution
dotnet build RubroX.slnx

# from root run individual projects as needed, e.g.:
dotnet run --project src/RubroX.API/RubroX.API.csproj
```

The Angular frontend can be started under `frontend/rubrox-ui`:

```bash
cd frontend/rubrox-ui
npm install
npm start
```

## Docker

The solution includes Dockerfiles for API, identity, and frontend. Use the provided `docker-compose.yml` at the root to orchestrate services.

## Testing

Run tests with:

```bash
dotnet test RubroX.slnx
```

## Notes

- Ensure the database is initialized using `infra/init-db.sql` before running.
- Configuration settings are located in `appsettings.json` files for each project.
