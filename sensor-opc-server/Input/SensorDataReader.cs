namespace sensor_opc_server.Input
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using sensor_opc_server.Interfaces;
    using sensor_opc_server.Interfaces.EventArgs;
    using sensor_opc_server.Models;
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
        public async Task ReadSensorDataAsync(CancellationToken ct)
        {
            try
            {
                using var streamReader = new StreamReader(_settingsConfiguration.SensorPath);
                while(true)
                {
                    ct.ThrowIfCancellationRequested();

                    var line = await streamReader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        ParseMessage(line);
                    }
                    await Task.Delay(50, ct);
                };
            }
            catch (OperationCanceledException oce)
            {
                if (oce.CancellationToken == ct)
                {
                    _logger.Information("Stopped reading file {path}", _settingsConfiguration.SensorPath);
                    return;
                }
                throw;
            }
            catch(Exception e)
            {
                _logger.Error(e, "Unhanled error while reading file {path}", _settingsConfiguration.SensorPath);
            }
        }

        private void ParseMessage(string line)
        {
            if(string.IsNullOrWhiteSpace(line))
            {
                _logger.Debug("Empty message received from sensor");
                return;
            }
            var parts = line.Split('|');
            if (parts.Length < 3)
            {
                _logger.Debug("Message received from sensor with unexpected format: {message}", line);
                return;
            }

            var typeOfMessage = parts[0];
            var versionOfMessage = parts[1];

            if (typeOfMessage == "t" && versionOfMessage == "v1.0")
            {
                ParseTelemetryV1(parts);
            } 
            else if (typeOfMessage == "d" && versionOfMessage == "v1.0")
            {
                ParseDiagnosticV1(parts);
            }
            else
            {
                _logger.Error("Can't parse message {Type} {Version} {Message}", typeOfMessage, versionOfMessage, line);
            }
        }

        private void ParseTelemetryV1(string[] parts)
        {
            var timestampString = parts[2];
            var identifier = parts[3];
            var payload = parts[4];

            if(float.TryParse(payload, out var value) && ulong.TryParse(timestampString, out var timestamp))
            {
                var telemetryMessage = new TelemetryMessageModelV1
                {
                    Timestamp = timestamp,
                    Identifier = identifier,
                    Value = value
                };

                TelemetryReceived?.Invoke(this, new TelemetryReceivedEventArgs(telemetryMessage));
            }
            else
            {
                _logger.Error("Can't parse telemetry v1.0 message {timestamp} {identifier} {value}",
                    timestampString,
                    identifier,
                    payload);
            }
        }

        private void ParseDiagnosticV1(string[] parts)
        {
            throw new NotImplementedException();
        }
    }
}