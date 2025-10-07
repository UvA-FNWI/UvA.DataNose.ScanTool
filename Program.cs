using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UvA.DataNose.ScanTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) => 
                    services.AddHostedService<Monitor>())
                .RunConsoleAsync();
        }
    }
}
