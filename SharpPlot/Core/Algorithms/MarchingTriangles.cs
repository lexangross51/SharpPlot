using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using SharpPlot.Core.Mesh;
using SharpPlot.Objects;

namespace SharpPlot.Core.Algorithms;

public class IsolineBuilder
{
    private readonly Mesh.Mesh _mesh;
    private readonly double[] _values;
    private readonly int[] _binaryMap;
    private List<Edge> _edges;
    public readonly List<Point> Points;

    public IsolineBuilder(Mesh.Mesh mesh, double[] values)
    {
        _mesh = mesh;
        _values = values;
        _binaryMap = new int[_values.Length];
        Points = new List<Point>();
        _edges = new List<Edge>(3);
    }
    
    private void MakeBinaryMap(double threshold)
    {
        Array.Clear(_binaryMap);

        for (int i = 0; i < _values.Length; i++)
        {
            _binaryMap[i] = _values[i] < threshold ? 0 : 1;
        }
    }
    
    private int GetElementState(Element element)
    {
        _edges.Clear();
        var bin = element.Nodes.Select(node => _binaryMap[node]).ToArray();
        var state = bin[2] * 4 + bin[1] * 2 + bin[0] * 1;
        
        if (state is 0 or 7) return state;
        
        for (int i = 0; i < 3; i++)
        {
            if (bin[i] == 0) continue;
            
            for (int j = 0; j < 3; j++)
            {
                if (element.Edges[j].Contain(element.Nodes[i]))
                {
                    _edges.Add(element.Edges[j]);
                }
            }
        }

        _edges = _edges.GroupBy(edge => edge)
            .Where(item => item.Count() == 1)
            .SelectMany(g => g)
            .ToList();
        
        return state;
    }
    
    public void BuildIsolines(int levels)
    {
        double min = _values.Min();
        double max = _values.Max();
        double step = (max - min) / levels;

        for (int i = 0; i < levels + 1; i++)
        {
            double threshold = min + i * step;
            
            MakeBinaryMap(threshold);

            for (int j = 0; j < _mesh.ElementsCount; j++)
            {
                var state = GetElementState(_mesh.Element(j));
                
                if (state is 0 or 7) continue;

                var edge = _edges.First();
                var p1 = _mesh.Point(edge.Node1);
                var p2 = _mesh.Point(edge.Node2);
                var v1 = _values[edge.Node1];
                var v2 = _values[edge.Node2];
                Points.Add(MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold));
                
                edge = _edges.Last();                                                  
                p1 = _mesh.Point(edge.Node1);                                          
                p2 = _mesh.Point(edge.Node2);                                          
                v1 = _values[edge.Node1];                                               
                v2 = _values[edge.Node2];                                               
                Points.Add(MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold));
            }
        }
    }
}

public class IsobandBuilder
{
    private readonly Mesh.Mesh _mesh;
    private readonly double[] _values;
    private readonly int[] _binaryMap;
    private List<Edge> _edges;
    private double[] _valuesByPalette = default!;

    public List<Point> Points { get; }
    public List<Color4> Colors { get; }

    public IsobandBuilder(Mesh.Mesh mesh, double[] values)
    {
        _mesh = mesh;
        _values = values;
        _binaryMap = new int[_values.Length];
        Points = new List<Point>(mesh.PointsCount);
        Colors = new List<Color4>(mesh.PointsCount);
        _edges = new List<Edge>(3);
    }
    
    private void MakeBinaryMap(double threshold)
    {
        Array.Clear(_binaryMap);

        for (int i = 0; i < _values.Length; i++)
        {
            _binaryMap[i] = _values[i] < threshold ? 0 : 1;
        }
    }
    
    private int GetElementState(Element element)
    {
        _edges.Clear();
        var bin = element.Nodes.Select(node => _binaryMap[node]).ToArray();
        var state = bin[2] * 4 + bin[1] * 2 + bin[0] * 1;
        
        if (state is 0 or 7) return state;
        
        for (int i = 0; i < 3; i++)
        {
            if (bin[i] == 0) continue;
            
            for (int j = 0; j < 3; j++)
            {
                if (element.Edges[j].Contain(element.Nodes[i]))
                {
                    _edges.Add(element.Edges[j]);
                }
            }
        }

        _edges = _edges.GroupBy(edge => edge)
            .Where(item => item.Count() == 1)
            .SelectMany(g => g)
            .ToList();
        
        return state;
    }
    
    public void BuildIsobands(int levels, Palette.Palette palette)
    {
        _valuesByPalette = new double[palette.ColorsCount + 1];

        double min = _values.Min();
        double max = _values.Max();
        double stepByLevels = (max - min) / levels;
        double stepByPalette = (max - min) / palette.ColorsCount;

        for (int i = 0; i < palette.ColorsCount + 1; i++)
            _valuesByPalette[i] = min + i * stepByPalette;

        for (int i = 0; i < levels + 1; i++)
        {
            double threshold = min + i * stepByLevels;
            
            MakeBinaryMap(threshold);

            for (int j = 0; j < _mesh.ElementsCount; j++)
            {
                var nodes = _mesh.Element(j).Nodes;
                var state = GetElementState(_mesh.Element(j));
                
                if (state is 0 or 7) continue;
                
                var edge = _edges.First();
                var p1 = _mesh.Point(edge.Node1);
                var p2 = _mesh.Point(edge.Node2);
                var v1 = _values[edge.Node1];
                var v2 = _values[edge.Node2];
                var intersected1 = MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold);
                
                edge = _edges.Last();                                                  
                p1 = _mesh.Point(edge.Node1);                                          
                p2 = _mesh.Point(edge.Node2);                                          
                v1 = _values[edge.Node1];                                               
                v2 = _values[edge.Node2];
                var intersected2 = MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold);
                
                if (state is 1 or 2 or 4)
                {
                    var nodeWith1 = nodes.First(node => _binaryMap[node] == 1);
                    Points.Add(_mesh.Point(nodeWith1));
                    Points.Add(intersected1);
                    Points.Add(intersected2);

                    var value = (_values[nodeWith1] + threshold + threshold) / 3.0;
                    var color = ColorInterpolator.InterpolateColor(_valuesByPalette, value, palette);
                    Colors.Add(color);
                    Colors.Add(color);
                    Colors.Add(color);
                }
                // else
                // {
                //     var nodeWith0 = nodes.First(node => _binaryMap[node] == 0);
                // }
            }
        }
    }
}