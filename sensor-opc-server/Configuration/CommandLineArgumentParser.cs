namespace sensor_opc_server.Configuration
{
    using System;
    using System.CommandLine;
    using System.Threading.Tasks;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.CommandLine.Parsing;
    using sensor_opc_server.Interfaces;

    public class CommandLineArgumentParser : ICommandLineArgumentParser
    {
        private readonly ISettingsConfiguration _settingsConfiguration;
        private bool _isValid;
        public CommandLineArgumentParser(ISettingsConfiguration settingsConfiguration)
        {
            _settingsConfiguration = settingsConfiguration ?? throw new ArgumentNullException(nameof(settingsConfiguration));
        }

        /// <inheritdoc />
        async Task<bool> ICommandLineArgumentParser.Parse(string[] args)
        {
            _isValid = false;
            var command = new RootCommand
            {
                new Argument<string>(
                    "sensor-path", 
                    "The path to the device to read the sensor data")
            };

            command.Description = "Server is reading serial data from device and provide as OPC UA Server";
            command.Handler = CommandHandler.Create(
                (string sensorPath) =>
                {
                    _isValid = !string.IsNullOrWhiteSpace(sensorPath);
                    _settingsConfiguration.SensorPath = sensorPath;
                });

            await command.InvokeAsync(args);

            return _isValid;
        }
    }
}