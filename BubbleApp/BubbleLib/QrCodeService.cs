using Microsoft.Maui.Controls;
using QRCoder;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

namespace BubbleAppNew.BubbleLib;

public interface IQrCodeService
{
    Task<ImageSource> GenerateQrCode(string payload);
}

public class QrCodeService : IQrCodeService
{
    public async Task<ImageSource> GenerateQrCode(string payload)
    {
        try
        {
            Console.WriteLine("Starting QR code generation");

            // 1. Generate the QR Code
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(
                pixelsPerModule: 20,
                drawQuietZones: true
            );

            Console.WriteLine("QR code bytes generated");

            // 2. Convert the byte array to ImageSharp
            using var qrImage = Image.Load<Rgba32>(qrCodeBytes);
            Console.WriteLine("QR image loaded");

            // 3. Load in the Logo
            using var logoStream = await FileSystem.OpenAppPackageFileAsync("waffle_moments_logo.png");
            Console.WriteLine("Logo file loaded");

            using var logoImage = Image.Load<Rgba32>(logoStream);
            Console.WriteLine("Logo image decoded");

            // 4. Resize the logo
            int logoSize = qrImage.Width / 4;
            logoImage.Mutate(x => x.Resize(logoSize, logoSize));
            Console.WriteLine($"Logo resized to {logoSize}x{logoSize}");

            // 5. Calculate centered position
            var posX = (qrImage.Width - logoSize) / 2;
            var posY = (qrImage.Height - logoSize) / 2;
            Console.WriteLine($"Logo position calculated at ({posX}, {posY})");

            // 6. Draw the logo on the QR code
            qrImage.Mutate(ctx => ctx.DrawImage(logoImage, new Point(posX, posY), 1f));
            Console.WriteLine("Logo embedded on QR code");

            // 7. Export the final image
            using var finalStream = new MemoryStream();
            qrImage.SaveAsPng(finalStream);
            finalStream.Position = 0;
            Console.WriteLine("Final QR image stream ready");

            return ImageSource.FromStream(() => finalStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating QR Code: {ex}");
            return null;
        }
    }
}
