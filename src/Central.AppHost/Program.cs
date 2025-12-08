var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("centraldb");

var server = builder.AddProject("server", "../Central.Server/Central.Server.csproj")
    .WithReference(postgresdb);

builder.Build().Run();
