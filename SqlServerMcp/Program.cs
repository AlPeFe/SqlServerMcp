using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlServerMcp.Resources;
using SqlServerMcp.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services
.AddMcpServer()
.WithStdioServerTransport()
.WithResources<SqlResources>()
.WithTools<SqlServerTools>();

await builder.Build().RunAsync();
