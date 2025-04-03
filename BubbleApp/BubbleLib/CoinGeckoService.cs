using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BubbleLib.Services;


public interface ICoinGeckoService
{
    Task<decimal> GetSolanaEuroExchangeRateAsync();
}

public class CoinGeckoService : ICoinGeckoService
{
    private readonly HttpClient _httpClient;

    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetSolanaEuroExchangeRateAsync()
    {
        try
        {
            var url = "https://api.coingecko.com/api/v3/simple/price?ids=solana&vs_currencies=eur";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("API returned unsuccessful status.");

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var solanaPrice = root.GetProperty("solana").GetProperty("eur").GetDecimal();
            return solanaPrice;
        }
        catch
        {
            // fallback value
            return 120.00m;
        }
    }
}