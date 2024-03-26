using Microsoft.AspNetCore.Mvc;

namespace SearchAlgosApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchAlgosController : ControllerBase
{
    private readonly ILogger<SearchAlgosController> _logger;

    public SearchAlgosController(ILogger<SearchAlgosController> logger)
    {
        _logger = logger;
    }

    [HttpPost("DepthFirstSearch", Name = "DFS")]
    public ActionResult<IEnumerable<string>> DFS(Graph graph, string startVertex)
    {
        var visitingOrder = graph.DepthFirstSearch(startVertex);
        return Ok(graph.ViewGraph(visitingOrder, "DFS", startVertex));
    }
    
    [HttpPost("BreadthFirstSearch", Name = "BFS")]
    public ActionResult<IEnumerable<string>> BFS(Graph graph, string startVertex)
    {
        var visitingOrder = graph.BreadthFirstSearch(startVertex);
        return Ok(graph.ViewGraph(visitingOrder, "BFS", startVertex));
    }
    
    [HttpPost("DirectTransitiveCloser", Name = "DTC")]
    public ActionResult<IEnumerable<string>> DirectTransitiveCloser(Graph graph, string startVertex)
    {
        var relatedDepth = graph.DirectTransitiveCloser(startVertex);
        return Ok(relatedDepth);
    }
    
    [HttpPost("aaa", Name = "DTaaaC")]
    public ActionResult<IEnumerable<string>> DirectTraansitiveCloser(int n)
    {
        var c = 0;
        var i = 0;
        while (n>=0)
        {
            n = n - 2;
            c = c + n - 2;
            i++;
        }

        return Ok(c);
    }
    
}