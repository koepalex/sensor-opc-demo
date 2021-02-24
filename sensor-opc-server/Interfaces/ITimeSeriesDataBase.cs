namespace sensor_opc_server.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using sensor_opc_server.Models;

    public interface ITimeSeriesDataBase: IDisposable
    {
        /// <summary>
        /// Writes the telemetry message into time series database
        /// </summary>
        Task WriteDataPointAsync(TelemetryMessageModelV1 model);
    }
}