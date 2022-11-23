using CSF;
using CSF.Hosting;
using CSF.Samples.Hosting;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureCommandFramework<CommandFramework, CommandResolver>()
    .Build();

await host.RunAsync();