// See https://aka.ms/new-console-template for more information

using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SeriLogThemesLibrary;

var builder = Host
    .CreateDefaultBuilder(args)
    .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Seq("http://localhost:5341")
        .WriteTo.Console(theme: SeriLogCustomThemes.Theme2())
    )
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<BloggingContext>();
        services.AddScoped<IBloggingService, BloggingService>();
    });

using IHost host = builder.Build();
var bloggingService = host.Services.GetService<IBloggingService>();
// uncomment to give a try.

// bloggingService.GetWithQueryTags();
// bloggingService.GetWithExtraLogs();
// bloggingService.InsertWithoutLogScope();
// bloggingService.InsertWithLogScope();
// bloggingService.ExecuteRawQueryWithoutQueryTags();
bloggingService.ExecuteRawQueryWithQueryTags();

await host.RunAsync();


