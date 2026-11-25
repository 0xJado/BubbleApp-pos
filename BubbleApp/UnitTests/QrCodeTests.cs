using BubbleLib;
using Microsoft.Maui.Controls;

namespace UnitTests;

public class QrCodeTests
{ 
    private IQrCodeService _qrCodeService;

    [SetUp]
    public void Setup()
    { 
        _qrCodeService = new QrCodeService();
    }

    [Test]
    public void GenerateQrCodeImage()
    { 
        string payload = "solana:walletAddress?amount=1.23";
        var result = _qrCodeService.GenerateQrCode(payload);

        Assert.That(result, Is.Not.Null); 
        Assert.That(result, Is.InstanceOf<ImageSource>());
    }
    
    [Test]
    public void InvalidQrCodePayload()
    { 
        string? payload = null;
        var result = _qrCodeService.GenerateQrCode(payload);

        Assert.That(result, Is.Null);
    }
}