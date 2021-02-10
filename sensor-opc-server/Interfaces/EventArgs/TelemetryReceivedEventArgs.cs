namespace sensor_opc_server.Interfaces.EventArgs
{
    using System;
    using sensor_opc_server.Models;

    public class TelemetryReceivedEventArgs : EventArgs
    {
        public TelemetryReceivedEventArgs()
        {

        }

        public TelemetryReceivedEventArgs(TelemetryMessageModelV1 message)
        {
            Message = message;
        }

        /// <summary>
        /// Telemetry data received from Sensor
        /// </summary>
        public TelemetryMessageModelV1 Message { get; set; }
    }
}