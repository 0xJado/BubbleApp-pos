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
        // Arrange: simulate a failing API
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new CoinGeckoService(httpClient);

        // Act
        var result = await service.GetSolanaEuroExchangeRateAsync();

        // Assert
        Assert.That(result, Is.EqualTo(120.00m));
    }
}
