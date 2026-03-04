using System;

namespace PaintApp.Src;

public class Coord(int x, int y){
    public int X {get;} = x;
    public int Y {get;} = y;

    public override string ToString()
    {
        return "{" + X.ToString() + ", " + Y.ToString() + "}"; 
    }
}