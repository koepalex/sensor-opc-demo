namespace sensor_opc_server.DependencyInjection
{
    using Autofac;
    using sensor_opc_server.Interfaces;
    using sensor_opc_server.Configuration;
    using Serilog;
    using sensor_opc_server.Input;

    public class SensorOpcServerAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SettingsConfiguration>()
                .As<ISettingsConfiguration>()
                .SingleInstance();

            builder.RegisterType<CommandLineArgumentParser>()
                .As<ICommandLineArgumentParser>()
                .SingleInstance();

            builder.Register<ILogger>((c, p) => {
                return new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(c.Resolve<ISettingsConfiguration>().LogLevel)
                    .WriteTo.Console()
                    .WriteTo.File(
                        c.Resolve<ISettingsConfiguration>().LogFileName,
                        fileSizeLimitBytes: 1024 * 1024,
                        flushToDiskInterval: c.Resolve<ISettingsConfiguration>().LogFileFlushTimeSpanSec,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 2)
                    .CreateLogger();
            }).SingleInstance()
              .ExternallyOwned();

            builder.RegisterType<SensorDataReader>()
              .As<ISensorDataReader>()
              .InstancePerLifetimeScope();
        }
    }
}