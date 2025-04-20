using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BubbleLib;

/// Interface for retrieving the current exchange rate between Solana (SOL) and Euro (EUR).
public interface ICoinGeckoService
{
    /// Asynchronously fetches the current SOL/EUR exchange rate using the CoinGecko API.
    Task<decimal> GetSolanaEuroExchangeRateAsync();
}

/// Service implementation for accessing CoinGecko's public API to retrieve Solana exchange rates.
public class CoinGeckoService : ICoinGeckoService
{
    private readonly HttpClient _httpClient;

    /// Injects an HTTP client for API communication.
    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Fetches and parses the latest SOL to EUR rate. Falls back to 120.00 if the request fails.
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