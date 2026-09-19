using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BubbleLib;

/// <summary>
/// Interface for retrieving the current exchange rate between Solana (SOL) and Euro (EUR).
/// </summary>
public interface ICoinGeckoService
{
    /// <summary>
    /// Asynchronously fetches the current SOL/EUR exchange rate using the CoinGecko API.
    /// Falls back to the last known cached value if the live request fails.
    /// </summary>
    /// <returns>The current exchange rate as a <see cref="decimal"/>.</returns>
    Task<decimal> GetSolanaEuroExchangeRateAsync();
}

/// <summary>
/// Service implementation that connects to CoinGecko's public API to retrieve real-time
/// Solana-to-Euro exchange rates. Includes internal caching to handle API failures gracefully.
/// </summary>
public class CoinGeckoService : ICoinGeckoService
{
    private readonly HttpClient _httpClient;
    private decimal? _cachedRate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoinGeckoService"/> class.
    /// </summary>
    /// <param name="httpClient">An <see cref="HttpClient"/> used to communicate with the CoinGecko API.</param>
    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves the current SOL/EUR exchange rate from CoinGecko. If the API is unavailable or the
    /// request fails, it returns the last successfully cached rate. Throws an exception if no
    /// cache is available.
    /// </summary>
    /// <returns>The exchange rate of Solana to Euro as a <see cref="decimal"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the API call fails and no cached exchange rate is available.
    /// </exception>
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

            _cachedRate = solanaPrice;
            return solanaPrice;
        }
        catch
        {
            if (_cachedRate.HasValue)
                return _cachedRate.Value;

            throw new InvalidOperationException("Unable to retrieve exchange rate and no cached value available.");
        }
    }
}
