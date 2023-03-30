using BlazorApp1.Shared;

namespace BlazorApp1.Client.Service
{
    public interface IBotService
    {
        Task<StockObject> ProcessStockRequestAsync(string stock_code);
    }
}
