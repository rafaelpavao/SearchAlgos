using Microsoft.AspNetCore.Mvc;

namespace SearchAlgosApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchAlgosController : ControllerBase
{
    private readonly ILogger<SearchAlgosController> _logger;
    public IGraph Graph { get; set; }

    public SearchAlgosController(ILogger<SearchAlgosController> logger, IGraph graph)
    {
        _logger = logger;
        Graph = graph;
    }
    
    [HttpPost("EnterGraph", Name = "Graph Input")]
    public IActionResult EnterGraph(Graph graph, bool isDirected, int verticesCount)
    {
        if(graph.IsDirected != isDirected)
        {
            return BadRequest("Bad request. Graph is not directed as specified. Please enter the correct graph type.");
        }
        if (!isDirected)
        {
            graph.CreateUndirectedGraph();
            Graph.SetDirected(false);
        }
        Graph.SetDirected(true);
        Graph.SetVertices(graph.Vertices);
        if(Graph.GetVertexCount() != verticesCount)
        {
            return BadRequest("Graph does not have predefined vertices count. Please enter the correct number of vertices.");
        }
        return Ok(Graph.ViewGraph());
    }
    
    
    
    [HttpPost("AddVertex", Name = "Add Vertex")]
    public IActionResult AddVertex(List<Vertex> vertices)
    {
        Graph.AddVertices(vertices);
        return Ok(Graph.ViewGraph());
    }
    
    [HttpPost("RemoveVertex", Name = "Remove Vertex")]
    public IActionResult RemoveVertex(string vertexName)
    {
        if (!Graph.GraphHasVertex(vertexName))
        {
            return BadRequest("Vertex not found");
        }
        Graph.RemoveVertex(vertexName);
        return Ok(Graph.ViewGraph());
    }
    
    [HttpGet("IsConnected", Name = "Graph Is Connected")]
    public ActionResult<bool> IsConnected()
    {
        return Ok(Graph.IsConnected());
    }
    
    [HttpGet("StronglyConnectedComponents", Name = "Graph SCC")]
    public ActionResult<IEnumerable<string>> StronglyConnectedComponents()
    {
        if(Graph.IsConnected()) return BadRequest("Graph is already connected. No need to find SCC.");
        var scc = Graph.StronglyConnectedComponents();
        var response = new Dictionary<string, string>();
        foreach (var component in scc)
        {
            response.Add($"Sub-Graph {scc.IndexOf(component) + 1}", string.Join(", ", component));
        }
        return Ok(response);
    }

    [HttpPost("DepthFirstSearch", Name = "DFS")]
    public ActionResult<IEnumerable<string>> DFS(string startVertex)
    {
        var visitingOrder = Graph.DepthFirstSearch(startVertex);
        return Ok(Graph.ViewGraph(visitingOrder, "DFS", startVertex));
    }
    
    [HttpPost("BreadthFirstSearch", Name = "BFS")]
    public ActionResult<IEnumerable<string>> BFS(string startVertex)
    {
        var visitingOrder = Graph.BreadthFirstSearch(startVertex);
        return Ok(Graph.ViewGraph(visitingOrder, "BFS", startVertex));
    }
    
    [HttpPost("DirectTransitiveCloser", Name = "DTC")]
    public ActionResult<IEnumerable<string>> DirectTransitiveCloser(string startVertex)
    {
        var relatedDepth = Graph.DirectTransitiveCloser(startVertex);
        return Ok(relatedDepth);
    }
    
    [HttpPost("IndirectTransitiveCloser", Name = "ITC")]
    public ActionResult<IEnumerable<string>> IndirectTransitiveCloser(string startVertex)
    {
        var relatedDepth = Graph.IndirectTransitiveCloser(startVertex);
        return Ok(relatedDepth);
    }
    
    [HttpGet("ColorGraph", Name = "ColorGraph")]
    public ActionResult<IEnumerable<string>> ColorGraph()
    {
        Graph.ColorGraph();
        return Ok(Graph.ViewGraph());
    }
}