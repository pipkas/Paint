using System.Collections;

namespace PaintApp.Src;

public class Stamp(StampType stampType, int vertexCount, int radiusSize, int rotationDegrees, int thickness = 1) : Tool
{
    public StampType StampType {get; } = stampType;
    public int VertexCount {get; } = vertexCount;
    public int RadiusSize {get; } = radiusSize;
    public int RotationDegrees {get; } = rotationDegrees;
    public int Thickness {get;} = thickness;
    public override ToolType ToolType => ToolType.Stamp;
}