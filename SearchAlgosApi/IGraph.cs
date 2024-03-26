namespace SearchAlgosApi;

public interface IGraph
{
    List<string> ViewGraph();
    Dictionary<string, object> ViewGraph(List<string> visitingOrder, string algoUsed, string startVertex);
    List<string> BreadthFirstSearch(string startVertex);
    List<string> DepthFirstSearch(string startVertex);
    Dictionary<string,int> DirectTransitiveCloser(string startVertex);
    void CreateUndirectedGraph();
    void SetVertices(List<Vertex> vertices);
    void AddVertices(List<Vertex> vertices);
    void RemoveVertex(string vertexName);
    bool GraphHasVertex(string vertexName);
    int GetVertexCount();
    bool IsConnected();
    List<List<string>> StronglyConnectedComponents();
}