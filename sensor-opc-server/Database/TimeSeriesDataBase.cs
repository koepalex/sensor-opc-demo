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
        private readonly string Token = "EDGBhfzL5Wahqr-_v7OOQG1yu53JJXPOkfS0YzLLlQDMxbOAd5phc5PH60LquSuQQaCstacljgrsO_B_O-sMTQ==";
        private readonly HttpClient _client;
        private readonly string _writeUrl;
        private readonly ILogger _logger;

        public TimeSeriesDataBase(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = new HttpClient(); 
            
            var ip = "172.28.176.1"; //"localhost";
            var org = "air";
            var bucket = "telemetry";
            var precision = "ms";
            _writeUrl = $"https://{ip}:8086/api/v2/write?org={org}&bucket={bucket}&precision={precision}";
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
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                using(var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(_writeUrl);
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("Authorization", $"Token {Token}");
                    //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Token);
                    request.Content = new StringContent($"{model.Identifier},sensor=MQ135 value=\"{model.Value}\" {timestamp}");
                    using (var response = await _client.SendAsync(request))
                    {
                        _logger.Information("Send data to InfluxDB, {StatusCode} {Content}", response.StatusCode, response.Content);
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