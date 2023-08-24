using System;
using SharpPlot.Core.Algorithms;

namespace SharpPlot.Core.Mesh;

public class Element
{
    public int[] Nodes { get; }
    public Edge[] Edges { get; }

    public Element(int[] nodes)
    {
        Nodes = nodes;
        Edges = new Edge[3];
        Array.Sort(Nodes);
        
        MakeEdges();
    }

    private void MakeEdges()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            Edges[i] = new Edge(Nodes[i], Nodes[(i + 1) % Nodes.Length]);
        }
    }

    public void CopyEdges(out Edge[] copied)
    {
        copied = new Edge[Edges.Length];

        for (int i = 0; i < Edges.Length; i++)
        {
            copied[i] = Edges[i];
        }
    }
}