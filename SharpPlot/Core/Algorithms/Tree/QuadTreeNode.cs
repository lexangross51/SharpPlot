using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpPlot.Core.Geometry;

namespace SharpPlot.Core.Algorithms.Tree;

public class QuadTreeNode<T>(RectangleF rect) where T : IQuadStorable
{
    private const int MaxElementPerNode = 400;
    private readonly RectangleF _rectangle = rect;
    private readonly float _midX = rect.X + rect.Width * 0.5f;
    private readonly float _midY = rect.Y + rect.Height * 0.5f;
    private readonly List<T> _items = [];
    private QuadTreeNode<T>? _bottomLeft;
    private QuadTreeNode<T>? _bottomRight;
    private QuadTreeNode<T>? _topLeft;
    private QuadTreeNode<T>? _topRight;

    public bool Insert(T item, RectangleF rect)
    {
        if (!_rectangle.Contains(rect) && !_rectangle.IntersectsWith(rect)) 
            return false;
        
        if (_items.Count < MaxElementPerNode)
        {
            _items.Add(item);
            return true;
        }

        if (_bottomLeft == null)
        {
            Subdivide();
        }

        bool res1 = _bottomLeft!.Insert(item, rect);
        bool res2 = _bottomRight!.Insert(item, rect);
        bool res3 = _topLeft!.Insert(item, rect);
        bool res4 = _topRight!.Insert(item, rect);

        return res1 || res2 || res3 || res4;
    }
    
    private void Subdivide()
    {
        var halfWidth = _rectangle.Width * 0.5f;
        var halfHeight = _rectangle.Height * 0.5f;
        
        _bottomLeft = new QuadTreeNode<T>(new RectangleF(_rectangle.X, _rectangle.Y, halfWidth, halfHeight));
        _bottomRight = new QuadTreeNode<T>(new RectangleF(_midX, _rectangle.Y, halfWidth, halfHeight));
        _topLeft = new QuadTreeNode<T>(new RectangleF(_rectangle.X, _midY, halfWidth, halfHeight));
        _topRight = new QuadTreeNode<T>(new RectangleF(_midX, _midY, halfWidth, halfHeight));

        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            if ((_bottomLeft._rectangle.Contains(item.Bounds) || _bottomLeft._rectangle.IntersectsWith(item.Bounds)) &&
                (_bottomRight._rectangle.Contains(item.Bounds) || _bottomRight._rectangle.IntersectsWith(item.Bounds)) &&
                (_topLeft._rectangle.Contains(item.Bounds) || _topLeft._rectangle.IntersectsWith(item.Bounds)) &&
                (_topRight._rectangle.Contains(item.Bounds) || _topRight._rectangle.IntersectsWith(item.Bounds)))
                continue;

            var rect = item.Bounds;
            _bottomLeft.Insert(item, rect);
            _bottomRight.Insert(item, rect);
            _topLeft.Insert(item, rect);
            _topRight.Insert(item, rect);
            
            _items.RemoveAt(i--);
        }
    }

    public void Query(Point3D p, HashSet<T> result)
    {
        if (!_rectangle.Contains((float)p.X, (float)p.Y)) return;

        foreach (var item in _items.Where(item => item.Contains(p.X, p.Y)))
        {
            result.Add(item);
        }

        if (_bottomLeft == null) return;
        
        _bottomLeft!.Query(p, result);
        _bottomRight!.Query(p, result);
        _topLeft!.Query(p, result);
        _topRight!.Query(p, result);
    }

    public bool Remove(T item)
    {
        if (!_rectangle.Contains(item.Bounds) && !_rectangle.IntersectsWith(item.Bounds)) 
            return false;
        if (_items.Remove(item)) return true;
        if (_bottomLeft == null) return false;

        bool res1 = _bottomLeft!.Remove(item);
        bool res2 = _bottomRight!.Remove(item);
        bool res3 = _topLeft!.Remove(item);
        bool res4 = _topRight!.Remove(item);

        return res1 || res2 || res3 || res4;
    }

    public void CollectAll(HashSet<T> allItems)
    {
        foreach (var i in _items)
        {
            allItems.Add(i);
        }

        if (_bottomLeft == null) return;
        
        _bottomLeft.CollectAll(allItems);
        _bottomRight!.CollectAll(allItems);
        _topLeft!.CollectAll(allItems);
        _topRight!.CollectAll(allItems);
    }
}