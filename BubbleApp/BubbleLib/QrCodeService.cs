using System;
using System.IO;
using Microsoft.Maui.Controls;
using QRCoder;

namespace BubbleLib;

/// <summary>
/// Provides functionality to generate QR code images based on a string payload.
/// </summary>
public interface IQrCodeService
{
    /// <summary>
    /// Generates a QR code image from the specified string payload.
    /// </summary>
    /// <param name="payload">The string content to encode in the QR code.</param>
    /// <returns>An <see cref="ImageSource"/> containing the generated QR code image, or null if generation fails.</returns>
    ImageSource? GenerateQrCode(string? payload);
}

/// <summary>
/// Implementation of <see cref="IQrCodeService"/> that utilizes the QRCoder library to generate QR codes.
/// </summary>
public class QrCodeService : IQrCodeService
{
    /// <summary>
    /// Generates a QR code image using the QRCoder library.
    /// </summary>
    /// <param name="payload">The text to encode in the QR code.</param>
    /// <returns>An image source containing the QR code, or null if an error occurs during generation.</returns>
    public ImageSource? GenerateQrCode(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return null;

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