using System;
using System.Threading;
using Autofac;
using sensor_opc_server.DependencyInjection;

namespace sensor_opc_server
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a cancellation token we can pass around and set when the application wants to exit
            var shutdownTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                shutdownTokenSource.Cancel();
            };

            IContainer container = ConfigureDependencies();
            
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
