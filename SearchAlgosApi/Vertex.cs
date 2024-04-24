namespace SearchAlgosApi;

public class Vertex
{
    public string VertexName { get; set; }
    public List<string> Connections { get; set; }

    public int Degree() => Connections.Count;

    public Color? Color { get; set; }

    // public int SaturationDegree { get; set; }

    public List<string> View()
    {
        var vertexView = new List<string>();
        var connections = string.Join(", ", Connections);
        vertexView.Add($"{VertexName} -> {connections}");
        vertexView.Add($"Degree: {Degree()}");
        vertexView.Add($"{nameof(Color)} : {Color?.GetColorName()} {Color?.ColorName}");
        return vertexView;
    }
}