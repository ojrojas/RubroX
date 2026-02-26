// RubroX Aspire AppHost
// NOTE: Requires .NET Aspire 9+ NuGet packages.
// Add <IsAspireServiceProject>true</IsAspireServiceProject> to API and Identity .csproj files
// to enable typed Projects.* references.

using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL + pgvector (docker image)
var postgres = builder.AddPostgres("postgres")
    .WithImage("ankane/pgvector", "latest")
    .WithDataVolume("rubrox-postgres-data");

var identityDb = postgres.AddDatabase("identity-db", "identity");
var rubroxDb   = postgres.AddDatabase("rubrox-db", "rubrox");

// Para usar typed references, descomentar y agregar <IsAspireServiceProject>true</IsAspireServiceProject>
// en los .csproj de API e Identity:
//
// var identity = builder.AddProject<Projects.RubroX_Identity>("identity")
//     .WithReference(identityDb);
// var api = builder.AddProject<Projects.RubroX_API>("api")
//     .WithReference(rubroxDb)
//     .WithReference(identity);

// Angular dev server
builder.AddNpmApp("frontend", "../../frontend/rubrox-ui", "start")
    .WithHttpEndpoint(env: "PORT", port: 4200);

builder.Build().Run();
