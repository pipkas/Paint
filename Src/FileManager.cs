using System;
using System.IO;
using SkiaSharp;

namespace PaintApp.Src;

public static class FileManager
{
    public static void SaveToPng(Picture picture, Stream stream)
    {
        var info = new SKImageInfo(
            picture.Width,
            picture.Height,
            SKColorType.Rgba8888,
            SKAlphaType.Opaque);

        using var bitmap = new SKBitmap(info);
        CopyManager.Copy(picture.PixelsBuffer, bitmap, 
                        picture.Width, picture.Height, true);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        data.SaveTo(stream);
    }

    public static Picture LoadFromStream(Stream stream)
    {
        using var bitmap = SKBitmap.Decode(stream) 
            ?? throw new Exception("Impossible to load the file");

        var picture = new Picture(bitmap.Width, bitmap.Height);

        var info = new SKImageInfo(
            bitmap.Width,
            bitmap.Height,
            SKColorType.Rgba8888,
            SKAlphaType.Opaque);

        using var converted = new SKBitmap(info);

        bitmap.CopyTo(converted);

        CopyManager.Copy(
            picture.PixelsBuffer,
            converted,
            bitmap.Width,
            bitmap.Height, 
            false);

        return picture;
    }
}