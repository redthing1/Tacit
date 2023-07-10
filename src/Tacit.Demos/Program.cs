using CliFx;
using Microsoft.Extensions.DependencyInjection;
using Minlog;

// var logger = new Logger(Verbosity.Information);
var logger = new Logger(Verbosity.Trace);
logger.Sinks.Add(new Logger.ConsoleSink());

// Log.Logger.Information("Solaris DIB CLI v{Version}", typeof(Program).Assembly.GetName().Version);

return await new CliApplicationBuilder()
    .SetDescription("Demos for Tacit AI")
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(commandTypes => {
        // We use Microsoft.Extensions.DependencyInjection for injecting dependencies in commands
        var services = new ServiceCollection();

        services.AddSingleton(logger);

        // Register all commands as transient services
        foreach (var commandType in commandTypes)
            services.AddTransient(commandType);

        return services.BuildServiceProvider();
    })
    .Build()
    .RunAsync();