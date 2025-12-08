#:sdk Aspire.AppHost.Sdk@13.0.2
#:package Aspire.Hosting.PostgreSQL

#:project ./src/Central.Server/Central.Server.csproj

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("centraldb");

var server = builder.AddProject<Projects.Central_Server>("server")
    .WithReference(postgresdb);

builder.Build().Run();
