using System.ComponentModel;
using BlazorJavascriptIsolationExtensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SampleAppWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddJavascriptReferences(options =>
{
    options.MapNamespace("SampleAppWasm.Pages",
        (assembly, component, isExternalAssembly) => $"./Pages/{component}.razor.js");
});

await builder.Build().RunAsync();
