using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UvA.DataNose.ScanTool
{
    class Program
    {
        async static Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) => 
                    services.AddHostedService<Monitor>())
                .RunConsoleAsync();
        }
    }
}
