global using FastEndpoints;
global using FastEndpoints.Security;
global using FastEndpoints.Swagger;
global using Central.Server;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace Central.Server;

/// <summary>
/// Marker type for reflection purposes to identify this assembly.
/// </summary>
public abstract record Meta;