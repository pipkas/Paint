namespace PaintApp.Src;

public class Line(int thickness): Tool
{
    public int Thickness {get; } = thickness;

    public override ToolType ToolType => ToolType.Line;
}