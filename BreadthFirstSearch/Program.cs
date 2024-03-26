// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Graph graph = new Graph
{
    Vertices = new List<Vertex>
    {
        new ()
        {
            VertexName = "A",
            Connections = new List<string> { "B", "D", "E" }
        },
        new ()
        {
            VertexName = "B",
            Connections = new List<string> { "A", "C" }
        },
        new ()
        {
            VertexName = "C",
            Connections = new List<string> { "B", "E" }
        },
        new ()
        {
            VertexName = "D",
            Connections = new List<string> { "A", "E" }
        },
        new ()
        {
            VertexName = "E",
            Connections = new List<string> { "A", "D", "C" }
        }
    }
};
        
graph.BreadthFirstSearch(graph, graph.Vertices.FirstOrDefault()!.VertexName);

public class Vertex
{
    public string VertexName { get; set; }
    public List<string> Connections { get; set; }
}

public class Graph
{
    public List<Vertex> Vertices { get; set; }
        
    public void BreadthFirstSearch(Graph graph, string startVertex)
    {
        var visited = new List<string>();
        var queue = new Queue<string>();
        queue.Enqueue(startVertex);
        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            if (visited.Contains(vertex))
            {
                continue;
            }
            Console.WriteLine(vertex);
            visited.Add(vertex);
            var connections = graph.Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    queue.Enqueue(connection);
                }
            }
        }
    }
}