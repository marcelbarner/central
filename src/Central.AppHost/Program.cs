var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("centraldb");

// Use absolute path resolution for the project
var server = builder.AddProject<Projects.Central_Server>("server","")
    .WithReference(postgresdb);

builder.Build().Run();
