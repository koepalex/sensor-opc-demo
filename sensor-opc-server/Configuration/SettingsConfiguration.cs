namespace sensor_opc_server.Configuration
{
    using System;
    using System.IO;
    using Opc.Ua;
    using sensor_opc_server.Interfaces;
    using Serilog.Core;

    public class SettingsConfiguration : ISettingsConfiguration
    {
        /// <inheritdoc />
        string ISettingsConfiguration.SensorPath { get; set; }

        /// <inheritdoc />
        string ISettingsConfiguration.LogFileName { get; set; } = Path.Combine(_appDataPath, $"{Utils.GetHostName()}-publisher.log");
        
        /// <inheritdoc />
        TimeSpan ISettingsConfiguration.LogFileFlushTimeSpanSec { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc />
        LoggingLevelSwitch ISettingsConfiguration.LogLevel { get; set; } = new LoggingLevelSwitch();

        private static string _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SensorOpcServer");
    }
}