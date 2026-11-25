using BubbleLib;
using MimeKit;
using Moq;

namespace UnitTests;

public class EmailServiceTests
{
    private Mock<ISmtpClientWrapper> _smtpMock;
    private EmailService _emailService;

    [SetUp]
    public void Setup()
    {
        _smtpMock = new Mock<ISmtpClientWrapper>();
        _emailService = new EmailService(_smtpMock.Object, "test@example.com", "test-only-password");
    }

    [Test]
    public async Task SendReceiptViaSmtpWrapper()
    {
        // Arrange
        var order = new CompletedOrder
        {
            OrderId = "order-xyz",
            TotalEur = 8,
            TotalSol = 0.123m,
            Wallet = "wallet-address",
            Timestamp = DateTime.Now
        };

        var waffles = new List<BubbleWaffle>
        {
            new() { WaffleName = "Test Waffle", Quantity = 1 }
        };

        // Act
        await _emailService.SendReceiptAsync("test@example.com", order, waffles);

        // Assert
        _smtpMock.Verify(s => s.SendAsync(It.Is<MimeMessage>(msg =>
            msg.Subject != null &&
            msg.Subject.Contains("Quittung") &&
            msg.To.Mailboxes.Select(mailbox => mailbox.Address).FirstOrDefault() == "test@example.com"
        )), Times.Once);
    }
}
