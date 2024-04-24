namespace SearchAlgosApi;


public class Color
{
    public int ColorName { get; set; }
    public Colors GetColorName() => (Colors)ColorName;

    public Color(int color)
    {
        ColorName = color;
    }
}
public enum Colors
{
    Red,
    Blue,
    Green,
    Yellow,
    Orange,
    Purple,
    Pink,
    Brown,
    Black,
    White,
    Gray,
    Cyan,
    Magenta,
    Lime,
    Maroon,
    Navy,
    Olive,
    Teal,
    Silver,
    Gold,
    Beige,
    Indigo,
    Violet,
    Turquoise,
    Tan,
    Salmon,
    Plum,
    Peach,
    Lavender,
    Khaki
}