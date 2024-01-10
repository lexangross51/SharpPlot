using System.Collections.Generic;
using System.Drawing;
using SharpPlot.Core.Geometry;

namespace SharpPlot.Core.Algorithms.Tree;

public class QuadTree<T>(RectangleF rectangle) where T : IQuadStorable
{
    private readonly QuadTreeNode<T> _root = new(rectangle);

    public bool Insert(T item, RectangleF rect)
    {
        return _root.Insert(item, rect);
    }

    public void Query(Point3D p, HashSet<T> result)
    {
        _root.Query(p, result);
    }
    
    public bool Remove(T item)
    {
        return _root.Remove(item);
    }

    public IEnumerable<T> CollectAll()
    {
        var allItems = new HashSet<T>();
        _root.CollectAll(allItems);
        
        return allItems;
    }
}