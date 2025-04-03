using Microsoft.Maui.Controls;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;

namespace BubbleAppNew.BubbleLib;

public interface IQrCodeService
{
    ImageSource GenerateQrCode(string payload);
}

public class QrCodeService : IQrCodeService
{
    public ImageSource GenerateQrCode(string payload)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        
        return ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }
}