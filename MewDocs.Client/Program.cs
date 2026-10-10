using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MewDocs.Client;

// The browser half of the docs: only the /playground island is interactive, and it runs
// the Mew compiler in-process. Nothing else on the site needs WebAssembly.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<MewCompiler>();

await builder.Build().RunAsync();
