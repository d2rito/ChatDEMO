using BlazorApp1.Shared;
using System.Text.Json;
using System.Text;
using Blazored.LocalStorage.Serialization;
using System;
using Newtonsoft.Json;

namespace BlazorApp1.Client.Service
{
    public class BotService : IBotService
    {
        private readonly HttpClient _httpClient;


        public BotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StockObject> ProcessStockRequestAsync(string stock_code)
        {
            var response = await _httpClient.GetAsync($"api/Bot?stock_code={stock_code}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content != string.Empty)
                {
                    var stockobject = JsonConvert.DeserializeObject<StockObject>(content);
                    return stockobject;
                }
            }
            return null;
        }
    }
}
