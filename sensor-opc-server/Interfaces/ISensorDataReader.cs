namespace sensor_opc_server.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using sensor_opc_server.Interfaces.EventArgs;

    public interface ISensorDataReader
    {
        /// <summary>
        /// Run aync operation to read sensor data 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>awaitable tasks</returns>
        Task ReadSensorDataAsync(CancellationToken ct = default);

        /// <summary>
        /// Event that will be invoked when sensor data is received
        /// </summary>
        event EventHandler<TelemetryReceivedEventArgs> TelemetryReceived;

    }
}