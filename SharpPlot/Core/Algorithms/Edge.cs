namespace SharpPlot.Core.Algorithms;

public struct Edge
{
    public int Node1 { get; set; }
    public int Node2 { get; set; }

    public Edge(int node1, int node2)
    {
        Node1 = node1;
        Node2 = node2;
    }

    public Edge Flip()
    {
        (Node1, Node2) = (Node2, Node1);
        return this;
    }

    public bool Contain(int node)
    {
        return Node1 == node || Node2 == node;
    }

    public bool Equals(Edge other)
    {
        return Node1 == other.Node1 && Node2 == other.Node2 ||
               Node1 == other.Node2 && Node2 == other.Node1;
    }
}