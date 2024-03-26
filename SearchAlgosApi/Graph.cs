using System.Collections;
using Microsoft.AspNetCore.Routing.Tree;

namespace SearchAlgosApi;

public class Graph
{
    public List<Vertex> Vertices { get; set; }
    
    public List<string> ViewGraph()
    {
        return Vertices.Select(v => v.View()).ToList();
    }
    

    public Dictionary<string, object> ViewGraph(List<string> visitingOrder, string algoUsed, string startVertex)
    {
        var graphData =  new Dictionary<string, object>();
        
        graphData.Add("GraphRepresentation", ViewGraph());
        graphData.Add("VisitingOrder", string.Join(", ", visitingOrder));
        graphData.Add("AlgoUsed", algoUsed);
        graphData.Add("StartVertex", startVertex);
        
        return graphData;
    }
    
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

    public Dictionary<string,int> DirectTransitiveCloser(string startVertex)
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
    
    public Dictionary<string,int> IndirectTransitiveCloser(string startVertex)
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
}