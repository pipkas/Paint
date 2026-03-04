using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace PaintApp.Src;

public class PaintManager: IPainter
{
    public DrawSettings Settings {get;}

    public Picture Image {get; set;}

    public PaintManager()
    {
        Settings = new DrawSettings();
        Image = new Picture();
    }

    public void DrawLine(Coord start, Coord end, Line line)
    {
        if (line.Thickness == 1)
        {
            BresenhamLineAlgorithm(start, end);
            return;
        }
        
        DrawThickLineWithSkia(start, end, line);
    }

    public void PutStamp(Coord center, Stamp stamp)
    {
        switch (stamp.StampType)
        {
            case StampType.Polygon:
                CreatePolygon(stamp, center);
                break;
            case StampType.Star:
                CreateStar(stamp, center);
                break;
            default:
                throw new NotSupportedException($"Unknown stamp type: {stamp.StampType}");
        }
    }

    public void Filling(Coord seed)
    {
        var oldColor = Image.GetPixel(seed.X, seed.Y);
        var newColor = Settings.Color;
        
        if (oldColor.SequenceEqual(newColor))
            return;
        
        var stack = new Stack<(int x, int y)>();
        stack.Push((seed.X, seed.Y));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();
            
            if (Image.GetPixel(x, y).SequenceEqual(newColor))
                continue;
                
            var span = FindSpan(x, y, oldColor);
            FillSpan(span, newColor);
            
            CheckAdjacentRow(span, y + 1, oldColor, newColor, stack);
            
            CheckAdjacentRow(span, y - 1, oldColor, newColor, stack);
        }
    }

    private (int startX, int endX, int y) FindSpan(int x, int y, byte[] oldColor)
    {
        int left = x;
        while (Image.IsInBounds(left - 1, y) && 
            Image.GetPixel(left - 1, y).SequenceEqual(oldColor))
        {
            left--;
        }
        
        int right = x;
        while (Image.IsInBounds(right + 1, y) && 
            Image.GetPixel(right + 1, y).SequenceEqual(oldColor))
        {
            right++;
        }
        
        return (left, right, y);
    }

    private void FillSpan((int startX, int endX, int y) span, byte[] newColor)
    {
        for (int x = span.startX; x <= span.endX; x++)
        {
            Image.SetPixel(x, span.y, newColor);
        }
    }

    private void CheckAdjacentRow((int startX, int endX, int y) currentSpan, int nextY, 
                                   byte[] oldColor, byte[] newColor, Stack<(int x, int y)> stack)
    {
        if (!Image.IsInBounds(currentSpan.startX, nextY))
            return;
            
        int x = currentSpan.startX;
        
        while (x <= currentSpan.endX)
        {   
            if (Image.IsInBounds(x, nextY) && Image.GetPixel(x, nextY).SequenceEqual(oldColor))
            {
                stack.Push((x, nextY));
                
                var nextSpan = FindSpan(x, nextY, oldColor);
                x = nextSpan.endX + 1;
                continue;
            }
            x++;
        }
    }

    private void BresenhamLineAlgorithm(Coord start, Coord end)
    {
        var color = Settings.Color;
        int dx = end.X - start.X;
        int dy = end.Y - start.Y;
        int adx = Math.Abs(dx);
        int ady = Math.Abs(dy);

        if (dx >= 0 && dy >= 0 && adx >= ady)
        {
            var err = -adx;
            var y = start.Y;
            for (int x = start.X; x <= end.X; x++)
            {
                err += 2 * ady;
                if (err > 0)
                {
                    err -= 2 * adx;
                    y++;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx >= 0 && dy > 0 && ady > adx)
        {
            var err = -ady;
            var x = start.X;
            for (int y = start.Y; y <= end.Y; y++)
            {
                err += 2 * adx;
                if (err > 0)
                {
                    err -= 2 * ady;
                    x++;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx < 0 && dy > 0 && ady >= adx)
        {
            var err = -ady;
            var x = start.X;
            for (int y = start.Y; y <= end.Y; y++)
            {
                err += 2 * adx;
                if (err > 0)
                {
                    err -= 2 * ady;
                    x--;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx < 0 && dy >= 0 && adx > ady)
        {
            var err = -adx;
            var y = start.Y;
            for (int x = start.X; x >= end.X; x--)
            {
                err += 2 * ady;
                if (err > 0)
                {
                    err -= 2 * adx;
                    y++;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx < 0 && dy <= 0 && adx >= ady)
        {
            var err = -adx;
            var y = start.Y;
            for (int x = start.X; x >= end.X; x--)
            {
                err += 2 * ady;
                if (err > 0)
                {
                    err -= 2 * adx;
                    y--;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx <= 0 && dy < 0 && ady > adx)
        {
            var err = -ady;
            var x = start.X;
            for (int y = start.Y; y >= end.Y; y--)
            {
                err += 2 * adx;
                if (err > 0)
                {
                    err -= 2 * ady;
                    x--;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx > 0 && dy < 0 && ady >= adx)
        {
            var err = -ady;
            var x = start.X;
            for (int y = start.Y; y >= end.Y; y--)
            {
                err += 2 * adx;
                if (err > 0)
                {
                    err -= 2 * ady;
                    x++;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
        else if (dx > 0 && dy <= 0 && adx > ady)
        {
            var err = -adx;
            var y = start.Y;
            for (int x = start.X; x <= end.X; x++)
            {
                err += 2 * ady;
                if (err > 0)
                {
                    err -= 2 * adx;
                    y--;
                }
                if (Image.IsInBounds(x, y))
                    Image.SetPixel(x, y, color);
            }
        }
    }

    private void DrawThickLineWithSkia(Coord start, Coord end, Line line)
    {
        var info = new SKImageInfo(
            Image.Width, Image.Height, 
            SKColorType.Rgba8888, SKAlphaType.Opaque);
        
        using var bitmap = new SKBitmap(info);

        CopyManager.Copy(Image.PixelsBuffer, bitmap, Image.Width, Image.Height, true);
        
        using var canvas = new SKCanvas(bitmap);
        using var paint = new SKPaint
        {
            Color = new SKColor(
                Settings.Color[0], Settings.Color[1], 
                Settings.Color[2], Settings.Color[3]),
            StrokeWidth = line.Thickness,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };
        
        canvas.DrawLine(start.X, start.Y, end.X, end.Y, paint);
        canvas.Flush();
        
        CopyManager.Copy(Image.PixelsBuffer,bitmap, Image.Width, Image.Height, false);
    }

    private void CreatePolygon(Stamp stamp, Coord center)
    {
        var angleStep = 2f * MathF.PI / stamp.VertexCount;
        var angleRad = stamp.RotationDegrees * MathF.PI / 180f;
        var vertexes = new List<Coord>(stamp.VertexCount);

        for (int i = 0; i < stamp.VertexCount; i++)
        {
            var angle = angleRad + i * angleStep;

            var x = center.X - stamp.RadiusSize * MathF.Sin(angle);
            var y = center.Y - stamp.RadiusSize * MathF.Cos(angle);
            vertexes.Add(new Coord((int)x, (int)y));
        }
        ConnectDots(vertexes);
    }

    private void CreateStar(Stamp stamp, Coord center)
    {
        var totalPoints = stamp.VertexCount * 2;
        var points = new List<Coord>(totalPoints);

        var angleStep = 2f * MathF.PI / totalPoints;
        var angleRad = stamp.RotationDegrees * MathF.PI / 180f;

        for (int i = 0; i < totalPoints; i++)
        {
            var angle = angleRad + i * angleStep;

            var radius = (i % 2 == 0)
                ? stamp.RadiusSize
                : stamp.RadiusSize / 3;

            var x = center.X - radius * MathF.Sin(angle);
            var y = center.Y - radius * MathF.Cos(angle);

            points.Add(new Coord((int)x, (int)y));
        }
        ConnectDots(points);
    }

    private void ConnectDots(List<Coord> dots)
    {
        for (int i = 0; i < dots.Count - 1; i++)
        {
            BresenhamLineAlgorithm(dots[i], dots[i + 1]);
        }
        BresenhamLineAlgorithm(dots[0], dots[^1]);
    }
}