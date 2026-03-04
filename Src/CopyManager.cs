using System;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace PaintApp.Src;

public static class CopyManager
{
    public static void Copy(byte[] buffer, SKBitmap bitmap, int width, int height, bool fromBufferToBitmap)
    {
        var ptr = bitmap.GetPixels();
        var rowBytes = bitmap.Info.RowBytes;
        var pixelBytesPerRow = width * 4;
        
        unsafe
        {
            for (int y = 0; y < height; y++)
            {
                var bufferOffset = y * pixelBytesPerRow;
                var bitmapRowPtr = (byte*)ptr + y * rowBytes;
                
                if (fromBufferToBitmap)
                    Marshal.Copy(buffer, bufferOffset, (IntPtr)bitmapRowPtr, pixelBytesPerRow);
                
                else
                    Marshal.Copy((IntPtr)bitmapRowPtr, buffer, bufferOffset, pixelBytesPerRow);
            }
        }
    }
}