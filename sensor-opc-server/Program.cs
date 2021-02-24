using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using sensor_opc_server.DependencyInjection;
using sensor_opc_server.Interfaces;

namespace sensor_opc_server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create a cancellation token we can pass around and set when the application wants to exit
            var shutdownTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                shutdownTokenSource.Cancel();
            };

            IContainer container = ConfigureDependencies();

            var claParser = container.Resolve<ICommandLineArgumentParser>();
            if (!(await claParser.Parse(args)))
            {
                return;
            }

            var sensorDataReader = container.Resolve<ISensorDataReader>();
            using(var tsdb = container.Resolve<ITimeSeriesDataBase>()) 
            {
                sensorDataReader.TelemetryReceived += (sender, args) => {
                    _ = tsdb.WriteDataPointAsync(args.Message);
                };

                var sensorTask = sensorDataReader.ReadSensorDataAsync(shutdownTokenSource.Token);
                await sensorTask;
            }
        }

        /// <summary>
        /// Configures the dependency injection container.
        /// </summary>
        private static IContainer ConfigureDependencies()
        {
            // Register default dependencies in the application container.
            var builder = new ContainerBuilder();
            builder.RegisterModule<SensorOpcServerAutofacModule>();

            var appContainer = builder.Build();
            return appContainer;
        }
    }


}
