using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NReco.PdfRenderer;

namespace UvA.DataNose.ScanTool
{
    public class Monitor : IHostedService
    {
        HttpClient Client;
        PhysicalFileProvider Provider;
        ILogger<Monitor> Logger;
        IDisposable Current;
        string ArchivePath;

        public Monitor(IConfiguration config, ILogger<Monitor> logger)
        {
            Logger = logger;
            var target = config["TargetPath"] ?? "/files";
            ArchivePath = config["ArchivePath"] ?? "/archive";
            Client = new HttpClient
            {
                BaseAddress = new Uri(config["TargetHost"])
            };
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config["ApiKey"]);
            Provider = new PhysicalFileProvider(target);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var token = Provider.Watch("*.pdf");
            Current = token.RegisterChangeCallback(async o => await StartAsync(cancellationToken), default);
            await ProcessFiles();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Current?.Dispose();
        }

        private async Task ProcessFiles()
        {
            var conv = new PdfToTextConverter();
            var files = Provider.GetDirectoryContents("./").ToArray();
            Logger.LogInformation($"Processing {files.Length} files");
            foreach (var file in files)
            {
                Logger.LogInformation($"Sending {file.Name} to endpoint");
                try 
                {
                    var text = conv.GenerateText(file.PhysicalPath);
                    var bytes = File.ReadAllBytes(file.PhysicalPath);
                    var res = await Client.PostAsJsonAsync("Files", new 
                    {
                        FileName = file.Name,
                        Content = bytes,
                        Text = text,
                        Date = File.GetCreationTime(file.PhysicalPath)
                    });
                    if (res.IsSuccessStatusCode)
                    {
                        Logger.LogInformation($"Sent file {file.Name}: {res.StatusCode}");
                        File.Move(file.PhysicalPath, Path.Combine(ArchivePath, file.Name));
                    }
                    else
                        Logger.LogError($"Failed to send file {file.Name}: {res.StatusCode}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error sending {file.Name}");
                }
            }
        }
    }
}
