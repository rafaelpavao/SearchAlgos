namespace SearchAlgosApi;

public class Vertex
{
    public string VertexName { get; set; }
    public List<string> Connections { get; set; }

    public string View()
    {
        var connections = string.Join(", ", Connections);
        return $"{VertexName} -> {connections}";
    }
}