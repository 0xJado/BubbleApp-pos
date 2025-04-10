using System;
using System.IO;
using Microsoft.Maui.Controls;
using QRCoder;

namespace BubbleLib;

public interface IQrCodeService
{
    ImageSource GenerateQrCode(string payload);
}

public class QrCodeService : IQrCodeService
{
    public ImageSource GenerateQrCode(string payload)
    {
        try
        {
            // Generate QR code data
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