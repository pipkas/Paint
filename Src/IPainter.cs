namespace PaintApp.Src;

public interface IPainter
{
    public void DrawLine(Coord start, Coord end, Line line);

    public void PutStamp(Coord center, Stamp stamp);

    public void Filling(Coord seed);
}