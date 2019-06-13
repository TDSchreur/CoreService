using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Topshelf;
using Topshelf.Autofac;

namespace Demoservice
{
    class Program
    {
        static int Main()
        {
            Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .WriteTo.Console(
                             LogEventLevel.Verbose,
                             "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                             theme: AnsiConsoleTheme.Literate)
                         .CreateLogger();

            return (int) HostFactory.Run(windowsService =>
            {
                windowsService.Service<MyServiceControl>(settings =>
                {
                    settings.ConstructUsingAutofacContainer();
                    settings.WhenStarted((service, control) => service.Start(control));
                    settings.WhenStopped((service, control) => service.Stop(control));
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();
                windowsService.UseAutofacContainer(CreateContainer());
                windowsService.UseSerilog(Log.Logger);
                windowsService.SetDescription("DemoService");
                windowsService.SetDisplayName("DemoService");
                windowsService.SetServiceName("DemoService");
            });
        }

        private static IContainer CreateContainer()
        {
            var services = new ServiceCollection()
                .AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(Log.Logger); });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<MyServiceControl>();
            builder.RegisterType<MyService>();

            var container = builder.Build();

            return container;
        }
    }
}