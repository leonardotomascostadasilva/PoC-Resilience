using Polly.Registry;

namespace PoC_Resilience
{
    public class WeatherService(
        IHttpClientFactory httpClientFactory,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            try
            {
                var url = $"https://api.opeweathermap.org/data/2.5/weather?q={city}";

                var httpClient = httpClientFactory.CreateClient();

                var pipeline = pipelineProvider.GetPipeline("default");

                var weatherResponse = await pipeline.ExecuteAsync(async ct => await httpClient.GetAsync(url, ct));


                if (!weatherResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                return await weatherResponse.Content.ReadFromJsonAsync<WeatherResponse>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

}
