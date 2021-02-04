namespace sensor_opc_server.Models
{
    public class TelemetryMessageModelV1
    {
        /// <summary>
        /// Milliseconds since start of sensor program
        /// </summary>
        ulong Timestamp { get; set; }
        
        /// <summary>
        /// Name of the telemetry data point
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// Vale of the telemetry data point
        /// </summary>
        float Value { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Timestamp}-{Identifier}:{Value}";
        }
    }
}