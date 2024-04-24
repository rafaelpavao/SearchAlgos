using System.Collections;
using Microsoft.AspNetCore.Routing.Tree;

namespace SearchAlgosApi;

public class Graph : IGraph
{
    public List<Vertex> Vertices { get; set; }
    public bool IsDirected { get; set; }

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

        relatedDepth.Add(startVertex, 0);
        visited.Add(startVertex);
        queue.Enqueue(startVertex);

        while (queue.Count > 0)
        {
            var currentVertex = queue.Dequeue();
            var currentDepth = relatedDepth[currentVertex];

            foreach (var connection in Vertices.FirstOrDefault(v => v.VertexName == currentVertex)!.Connections)
            {
                if (!visited.Contains(connection))
                {
                    relatedDepth.Add(connection, currentDepth + 1);
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

        stack.Push(startVertex);
        relatedDepth.Add(startVertex, 0);

        while (stack.Count > 0)
        {
            var currentVertex = stack.Pop();
            var currentDepth = relatedDepth[currentVertex];

            if (!visited.Contains(currentVertex))
            {
                visited.Add(currentVertex);

                foreach (var vertex in Vertices)
                {
                    if (vertex.Connections.Contains(currentVertex) && !visited.Contains(vertex.VertexName))
                    {
                        stack.Push(vertex.VertexName);
                        relatedDepth.Add(vertex.VertexName, currentDepth + 1);
                    }
                }
            }
        }

        return relatedDepth;
    }

    #endregion

    #region GraphUtils

    public List<List<string>> ViewGraph()
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

    public void SetDirected(bool isDirected)
    {
        IsDirected = isDirected;
    }

    public void AddVertices(List<Vertex> vertices)
    {
        foreach (var vertex in vertices)
        {
            bool willAdd = !Vertices.Select(v => v.VertexName).Contains(vertex.VertexName);

            if (vertex.Connections.Count > 0)
            {
                if (vertex.Connections.Except(Vertices.Select(v => v.VertexName)).Any())
                {
                    if (VertexConnectsOnlyToHimself(vertex)) willAdd = true;
                    else willAdd = false;
                }
            }

            if (willAdd) Vertices.Add(vertex);
            if (!IsDirected)
            {
                foreach (var connection in vertex.Connections)
                {
                    var connectedVertex = Vertices.FirstOrDefault(v => v.VertexName == connection);
                    if (connectedVertex != null && !connectedVertex.Connections.Contains(vertex.VertexName))
                    {
                        connectedVertex.Connections.Add(vertex.VertexName);
                    }
                }
            }
        }
    }

    private bool VertexConnectsOnlyToHimself(Vertex vertex)
    {
        return vertex.Connections.Count is 1 && vertex.Connections.First().Equals(vertex.VertexName);
    }

    public void RemoveVertex(string vertexName)
    {
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
        foreach (var vertex in Vertices)
        {
            var directClosure = DirectTransitiveCloser(vertex.VertexName).Select(x => x.Key);
            var indirectClosure = IndirectTransitiveCloser(vertex.VertexName).Select(x => x.Key);

            var directSet = new HashSet<string>(directClosure);
            var indirectSet = new HashSet<string>(indirectClosure);

            if (!directSet.SetEquals(indirectSet) || directSet.Count != Vertices.Count)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region StronglyConnectedComponents

    public List<List<string>> StronglyConnectedComponents()
    {
        var stronglyConnectedComponents = new List<List<string>>();
        var visited = new HashSet<string>();

        foreach (var vertex in Vertices)
        {
            if (!visited.Contains(vertex.VertexName))
            {
                var directClosure = DirectTransitiveCloser(vertex.VertexName).Select(x => x.Key);
                var indirectClosure = IndirectTransitiveCloser(vertex.VertexName).Select(x => x.Key);

                var intersection = directClosure.Intersect(indirectClosure).ToList();

                if (intersection.Any())
                {
                    stronglyConnectedComponents.Add(intersection);
                    visited.UnionWith(intersection);
                }
            }
        }

        return stronglyConnectedComponents;
    }

    #endregion

    private int GetSaturationDegreeOfVertices(Vertex vertex)
    {
        var adjacentColors = new List<Color>();
        foreach (var connection in vertex.Connections)
        {
            var connectedVertex = Vertices.FirstOrDefault(v => v.VertexName == connection);
            if(connectedVertex?.Color is null) continue;
            if (!adjacentColors.Contains(connectedVertex.Color))
            {
                adjacentColors.Add(connectedVertex.Color);
            }
        }

        return adjacentColors.Count;
    }
    
    public void ColorGraph()
    {
        var saturationOrder = Vertices.OrderByDescending(v => v.Degree()).ToList();
        var i = 1;
        while(Vertices.Any(v => v.Color == null))
        {
            foreach (var vertex in saturationOrder.Where(v => v.Color == null))
            {
                var connectedVertices = 
                    vertex.Connections.Select(c => Vertices.FirstOrDefault(v => v.VertexName == c)).ToList();
                if (connectedVertices.All(cv => cv?.Color?.ColorName != i))
                {
                    vertex.Color = new Color(i);
                    saturationOrder = saturationOrder.OrderByDescending(v => GetSaturationDegreeOfVertices(v)).ThenByDescending(v => v.Degree()).ToList();
                }
            }
            i++;
        }
    }
}