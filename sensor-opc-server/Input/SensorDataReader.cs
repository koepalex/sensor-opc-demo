namespace sensor_opc_server.Input
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using sensor_opc_server.Interfaces;
    using sensor_opc_server.Interfaces.EventArgs;
    using Serilog;

    public class SensorDataReader : ISensorDataReader
    {
        private readonly ILogger _logger;
        private readonly ISettingsConfiguration _settingsConfiguration;

        public SensorDataReader(ILogger logger, ISettingsConfiguration settingsConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingsConfiguration = settingsConfiguration ?? throw new ArgumentNullException(nameof(settingsConfiguration));
        }
        
        /// <inheritdoc />
        public event EventHandler<TelemetryReceivedEventArgs> TelemetryReceived;

        /// <inheritdoc />
        public Task ReadSensorDataAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}