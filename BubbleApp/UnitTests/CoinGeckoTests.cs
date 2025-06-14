using BubbleLib;
using Moq;
using Moq.Protected;
using System.Net;

namespace UnitTests;

public class CoinGeckoServiceTests
{
    private const string CoinGeckoMockResponse = """
    {
        "solana": {
            "eur": 98.00
        }
    }
    """;
    
    private const string CachedCoinGeckoMockResponse = """
    {
        "solana": {
            "eur": 120.00
        }
    }
    """;

    [Test]
    public async Task SuccessfulSolanaEuroExchangeRate()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(CoinGeckoMockResponse)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new CoinGeckoService(httpClient);

        // Act
        var rate = await service.GetSolanaEuroExchangeRateAsync();

        // Assert
        Assert.That(rate, Is.EqualTo(98.00m));
    }

    [Test]
    public async Task ExchangeRateApiErrorFallback()
    {
        // Arrange: simulate a failure on the second call for cache
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // First call succeeds
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(CachedCoinGeckoMockResponse)
            })
            // Second call fails
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new CoinGeckoService(httpClient);

        // Act
        var _ = await service.GetSolanaEuroExchangeRateAsync(); // primes cache with 120.00
        var result = await service.GetSolanaEuroExchangeRateAsync(); // uses fallback

        // Assert
        Assert.That(result, Is.EqualTo(120.00m));
    }
}