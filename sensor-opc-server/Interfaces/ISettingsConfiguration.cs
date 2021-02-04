namespace sensor_opc_server.Interfaces
{
    using System;
    using Serilog.Core;

    public interface ISettingsConfiguration
    {
        /// <summary>
        /// Gets or sets the path to access the serial data from sensor 
        /// </summary>
        string SensorPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        string LogFileName { get; set; }

        /// <summary>
        /// Gets or sets the flushing interval to write log file to disk
        /// </summary>
        TimeSpan LogFileFlushTimeSpanSec { get; set; }

        /// <summary>
        /// Gets or sets the logging level
        /// </summary>
        LoggingLevelSwitch LogLevel { get; set; }
    }
}