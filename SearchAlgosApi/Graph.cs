using System.Collections;
using Microsoft.AspNetCore.Routing.Tree;

namespace SearchAlgosApi;

public class Graph : IGraph
{
    public List<Vertex> Vertices { get; set; }

    #region BFS

    public List<string> BreadthFirstSearch(string startVertex)
    {
        var queue = new Queue<string>();
        queue.Enqueue(startVertex);
        return BFSVisitingAlgo(queue);
    }

    private List<string> BFSVisitingAlgo(Queue<string> queue)
    {
        var visitingOrder = new List<string>();
        var visited = new HashSet<string>();
        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            visitingOrder.Add(vertex);
            visited.Add(vertex);
            var connections = Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    if (!visited.Contains(connection))
                    {
                        queue.Enqueue(connection);
                        visited.Add(connection);
                    }
                }
            }
        }

        return visitingOrder;
    }

    #endregion
    
    #region DFS

    public List<string> DepthFirstSearch(string startVertex)
    {
        var stack = new Stack<string>();
        stack.Push(startVertex);
        var visitingOrder = DFSVisitingAlgo(stack);
        return visitingOrder;
    }

    private List<string> DFSVisitingAlgo(Stack<string> stack)
    {
        var visitingOrder = new List<string>();
        var visited = new HashSet<string>();
        while (stack.Count > 0 || Vertices.Select(v => v.VertexName).Except(visited).Any())
        {
            if (stack.Count == 0)
            {
                stack.Push(Vertices.Select(v => v.VertexName).Except(visited).First());
            }

            var vertex = stack.Peek();
            if (visited.Contains(vertex))
            {
                stack.Pop();
                continue;
            }

            visitingOrder.Add(vertex);
            visited.Add(vertex);
            var connections = Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    var connectedVertex = Vertices.FirstOrDefault(v => v.VertexName == connection);
                    if (connectedVertex != null && !visited.Contains(connectedVertex.VertexName))
                    {
                        stack.Push(connectedVertex.VertexName);
                    }
                }
            }
        }

        return visitingOrder;
    }

    #endregion

    #region TransitiveClosure

    public Dictionary<string, int> DirectTransitiveCloser(string startVertex)
    {
        var relatedDepth = new Dictionary<string, int>();
        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        int depth = 0;

        relatedDepth.Add(startVertex, depth);
        visited.Add(startVertex);
        queue.Enqueue(startVertex);

        while (queue.Count > 0)
        {
            var currentVertex = queue.Dequeue();
            depth = relatedDepth[currentVertex] + 1;

            foreach (var connection in Vertices.FirstOrDefault(v => v.VertexName == currentVertex)!.Connections)
            {
                if (!visited.Contains(connection))
                {
                    relatedDepth.Add(connection, depth);
                    visited.Add(connection);
                    queue.Enqueue(connection);
                }
            }
        }

        return relatedDepth;
    }

    public Dictionary<string, int> IndirectTransitiveCloser(string startVertex)
    {
        var relatedDepth = new Dictionary<string, int>();
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        int depth = 0;

        relatedDepth.Add(startVertex, depth);
        // visited.Add(startVertex);
        stack.Push(startVertex);

        while (stack.Count > 0)
        {
            var currentVertex = stack.Pop();
            depth = relatedDepth[currentVertex] + 1;

            foreach (var connection in Vertices.FirstOrDefault(v => v.VertexName == currentVertex)!.Connections)
            {
                if (!visited.Contains(connection))
                {
                    relatedDepth.Add(connection, depth);
                    visited.Add(connection);
                    stack.Push(connection);
                }
            }
        }

        return relatedDepth;
    }

    #endregion
    
    #region GraphUtils

    public List<string> ViewGraph()
    {
        return Vertices.Select(v => v.View()).ToList();
    }


    public Dictionary<string, object> ViewGraph(List<string> visitingOrder, string algoUsed, string startVertex)
    {
        var graphData = new Dictionary<string, object>();

        graphData.Add("GraphRepresentation", ViewGraph());
        graphData.Add("VisitingOrder", string.Join(", ", visitingOrder));
        graphData.Add("AlgoUsed", algoUsed);
        return graphData;
    }
    public void CreateUndirectedGraph()
    {
        foreach (var vertex in Vertices)
        {
            var connectionsCopy = new List<string>(vertex.Connections);
            foreach (var connection in connectionsCopy)
            {
                var connectedVertex = Vertices.FirstOrDefault(v => v.VertexName == connection);
                if (connectedVertex != null && !connectedVertex.Connections.Contains(vertex.VertexName))
                {
                    connectedVertex.Connections.Add(vertex.VertexName);
                }
            }
        }
    }

    public void SetVertices(List<Vertex> vertices)
    {
        Vertices = vertices;
    }

    public void AddVertices(List<Vertex> vertices)
    {
        foreach (var vertex in vertices)
        {
            if (Vertices.Select(v => v.VertexName).Contains(vertex.VertexName))
            {
                continue;
            }

            if (vertex.Connections.Count > 0)
            {
                if (vertex.Connections.Except(Vertices.Select(v => v.VertexName)).Any())
                {
                    continue;
                }
            }

            Vertices.Add(vertex);
        }
    }

    public void RemoveVertex(string vertexName)
    {
        // 1. vertex exists in the graph
        // 2. remove vertex from the graph
        // 3. remove vertex from the connections of other vertices

        if (!Vertices.Select(v => v.VertexName).Contains(vertexName))
        {
            return;
        }

        Vertices.RemoveAll(v => v.VertexName == vertexName);
        foreach (var vertex in Vertices)
        {
            vertex.Connections.Remove(vertexName);
        }
    }

    public bool GraphHasVertex(string vertexName)
    {
        return Vertices.Select(v => v.VertexName).Contains(vertexName);
    }

    public int GetVertexCount()
    {
        return Vertices.Count;
    }

    #endregion

    #region GraphIsConnected

    public bool IsConnected()
    {
        var stack = new Stack<string>();
        stack.Push(Vertices.First().VertexName);
        var visitingOrder = IsConnectedVisitingAlgo(stack);
        return visitingOrder;
    }

    private bool IsConnectedVisitingAlgo(Stack<string> stack)
    {
        var visited = new HashSet<string>();
        while (stack.Count > 0)
        {
            var vertex = stack.Pop();
            if (!visited.Contains(vertex))
            {
                visited.Add(vertex);
                var connections = Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
                if (connections != null)
                {
                    foreach (var connection in connections)
                    {
                        var connectedVertex = Vertices.FirstOrDefault(v => v.VertexName == connection);
                        if (connectedVertex != null && !visited.Contains(connectedVertex.VertexName))
                        {
                            stack.Push(connectedVertex.VertexName);
                        }
                    }
                }
            }
        }

        return visited.Count == Vertices.Count;
    }

    #endregion

    #region StronglyConnectedComponents

    public List<List<string>> StronglyConnectedComponents()
    {
        var stack = new Stack<string>();
        var visited = new HashSet<string>();

        // Primeira passagem DFS para determinar a ordem de término
        foreach (var vertex in Vertices)
        {
            if (!visited.Contains(vertex.VertexName))
            {
                DFSOrder(vertex.VertexName, visited, stack);
            }
        }

        // Inverter as direções de todas as arestas
        InvertGraph();

        visited.Clear();
        var stronglyConnectedComponents = new List<List<string>>();

        // Segunda passagem DFS para encontrar os componentes fortemente conexos
        while (stack.Count > 0)
        {
            var vertex = stack.Pop();
            if (!visited.Contains(vertex))
            {
                var component = new List<string>();
                DFSComponent(vertex, visited, component);
                stronglyConnectedComponents.Add(component);
            }
        }

        // Inverter as direções de todas as arestas novamente para restaurar o grafo original
        InvertGraph();

        return stronglyConnectedComponents;
    }

    private void DFSOrder(string vertex, HashSet<string> visited, Stack<string> stack)
    {
        visited.Add(vertex);
        var connections = Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
        if (connections != null)
        {
            foreach (var connection in connections)
            {
                if (!visited.Contains(connection))
                {
                    DFSOrder(connection, visited, stack);
                }
            }
        }

        stack.Push(vertex);
    }

    private void DFSComponent(string vertex, HashSet<string> visited, List<string> component)
    {
        visited.Add(vertex);
        component.Add(vertex);
        var connections = Vertices.FirstOrDefault(v => v.VertexName == vertex)?.Connections;
        if (connections != null)
        {
            foreach (var connection in connections)
            {
                if (!visited.Contains(connection))
                {
                    DFSComponent(connection, visited, component);
                }
            }
        }
    }

    private void InvertGraph()
    {
        var invertedConnections = new Dictionary<string, List<string>>();
        foreach (var vertex in Vertices)
        {
            foreach (var connection in vertex.Connections)
            {
                if (!invertedConnections.ContainsKey(connection))
                {
                    invertedConnections[connection] = new List<string>();
                }

                invertedConnections[connection].Add(vertex.VertexName);
            }
        }

        foreach (var vertex in Vertices)
        {
            vertex.Connections = invertedConnections.ContainsKey(vertex.VertexName)
                ? invertedConnections[vertex.VertexName]
                : new List<string>();
        }
    }

    #endregion

    
}