namespace sensor_opc_server.Database
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using sensor_opc_server.Interfaces;
    using sensor_opc_server.Models;
    using Serilog;

    public class TimeSeriesDataBase : ITimeSeriesDataBase
    {
        private readonly HttpClient _client;
        private readonly string _writeUrl;
        private readonly ILogger _logger;
        private ulong _counter;

        public TimeSeriesDataBase(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = new HttpClient();
            _counter = 0; 
            
            var ip = "localhost";
            var dbName = "db0";
            _writeUrl = $"http://{ip}:8086/write?db={dbName}";

            //todo create db if not exist
        }

        void IDisposable.Dispose()
        {
            _client.Dispose();
        }

        async Task ITimeSeriesDataBase.WriteDataPointAsync(TelemetryMessageModelV1 model)
        {
            _logger.Verbose("Received: {Identifier} {Value}", model.Identifier, model.Value);

            try 
            {
                var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                using(var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(_writeUrl);
                    request.Method = HttpMethod.Post;
                    var content = $"{model.Identifier},sensor=MQ135 value={model.Value.ToString("E5")} {timestamp}";
                    _logger.Verbose("TimeSeriesDB {Request}", content);
                    request.Content = new StringContent(content);
                    using (var response = await _client.SendAsync(request))
                    {
                        _counter++;
                        if (!response.IsSuccessStatusCode) {
                            _logger.Warning("Send data to InfluxDB, {StatusCode} {Content}", response.StatusCode, response.Content);
                        }
                    }

                    if ((_counter % 50) == 0)
                    {
                        _logger.Information("{TotalNumber} measurements added to Timeseries database", _counter);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "can't write to InfluxDB");
            }
        }
    }
}