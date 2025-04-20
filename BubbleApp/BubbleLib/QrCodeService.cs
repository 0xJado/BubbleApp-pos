using System;
using System.IO;
using Microsoft.Maui.Controls;
using QRCoder;

namespace BubbleLib;

/// Provides an interface for generating QR codes.
public interface IQrCodeService
{
    /// Generates a QR code image from the given payload string.
    ImageSource GenerateQrCode(string payload);
}

/// Service that uses the QRCoder library to generate QR code images.
public class QrCodeService : IQrCodeService
{
    /// Generates a QR code image from the given payload.
    public ImageSource GenerateQrCode(string payload)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);

            var qrCodeBytes = qrCode.GetGraphic(20);

            return ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating QR Code: {ex.Message}");
            return null;
        }
    }
}