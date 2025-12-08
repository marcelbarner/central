var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("centraldb");

// Use absolute path resolution for the project
var server = builder.AddProject<Projects.Central_Server>("server")
    .WithReference(postgresdb)
    .WithHttpsEndpoint(port: 5001, name: "https");

var client = builder.AddJavaScriptApp("client", Path.Combine("..", "Central.Client"), "start")
    .WithNpm()
    .WithReference(server)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();