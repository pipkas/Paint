using System;

namespace PaintApp.Src;

public class Picture
{
    public int Width;
    public int Height;

    //RGBA color
    public byte[] PixelsBuffer { get; private set;}

    public const int ColorBytesCount = 4;

    public Picture(int width = 550, int height = 400)
    {
        if (width == 0 || height == 0)
            throw new InvalidOperationException($"Picture has impossible size: {width}x{height}");

        Width = width;
        Height = height;
        PixelsBuffer = new byte[width * height * ColorBytesCount];
        Clean();
    }

    public void SetPixel(int x, int y, byte[] color)
    {
        CheckBounds(x, y);
        if (color == null || color.Length != 4)
            throw new ArgumentException("Invalid definition of color");

        var pos = ColorBytesCount * (Width * y + x);
        for (int i = 0; i < ColorBytesCount; i++)
            PixelsBuffer[pos + i] = color[i];
        
    }

    public byte[] GetPixel(int x, int y)
    {
        CheckBounds(x, y);
        var pos = ColorBytesCount * (Width * y + x);
        var color = new byte[ColorBytesCount];
        for (int i = 0; i < ColorBytesCount; i++)
            color[i] = PixelsBuffer[pos + i];
        
        return color;
    }

    public void Resize(int newWidth, int newHeight)
    {
        if (newWidth == 0 || newHeight == 0)
            throw new InvalidOperationException($"Picture has impossible size: {newWidth}x{newHeight}");
        
        var newBuffer = new byte[newWidth * newHeight * ColorBytesCount];
        Clean(newBuffer);
        CopyBuffer(PixelsBuffer, newBuffer, Width, Height, newWidth, newHeight);

        PixelsBuffer = newBuffer;
        Width = newWidth;
        Height = newHeight;
    }
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public void CheckBounds(int x, int y)
    {
        if (!IsInBounds(x, y))
            throw new IndexOutOfRangeException($"point ({x}, {y}) is out of bounds [{Width}, {Height}]");
    }

    public void Clean()  => Array.Fill(PixelsBuffer, (byte)255);
    private void Clean(byte[] buffer) => Array.Fill(buffer, (byte)255);

    private void CopyBuffer(byte[] oldBuffer, byte[] newBuffer, int oldWidth, int oldHeight, int newWidth, int newHeight)
    {
        if (oldBuffer == null || newBuffer == null || oldWidth >= newWidth || oldHeight >= newHeight) return;
        
        for (int y = 0; y < oldHeight; y++)
        {   
            Buffer.BlockCopy(
                oldBuffer,
                y * oldWidth * ColorBytesCount,
                newBuffer,
                y * newWidth * ColorBytesCount,
                oldWidth * ColorBytesCount);
        }
    }

}