﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SharpPlot.Core.Mesh;

namespace SharpPlot.Core.Algorithms;

public class IsolineBuilder
{
    private readonly Mesh.Mesh _mesh;
    private readonly double[] _values;
    private readonly int[] _binaryMap;
    private List<Edge> _edges;
    public readonly List<Isoline> Isolines;

    public IsolineBuilder(Mesh.Mesh mesh, double[] values)
    {
        _mesh = mesh;
        _values = values;
        _binaryMap = new int[_values.Length];
        Isolines = new List<Isoline>();
        _edges = new List<Edge>(3);
    }

    /// <summary>
    /// Make binary map: 1 - data value above threshold,
    ///                  0 - data value below threshold,
    /// </summary>
    /// <param name="threshold"></param>
    private void MakeBinaryMap(double threshold)
    {
        Array.Clear(_binaryMap);

        for (int i = 0; i < _values.Length; i++)
        {
            _binaryMap[i] = _values[i] < threshold ? 0 : 1;
        }
    }

    // 1------2
    // |------|
    // |------|
    // 4------3
    
    private int GetElementState(Element element)
    {
        _edges.Clear();
        var bin = element.Nodes.Select(node => _binaryMap[node]).ToArray();

        for (int i = 0; i < 3; i++)
        {
            if (bin[i] != 1) continue;
            
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
        
        return bin[2] * 4 + bin[1] * 2 + bin[0] * 1;
    }

    private Isoline? MakeIsolineByState(int state, double threshold)
    {
        if (state is 0 or 7) return null;

        var isoline = new Isoline();
        var edge = _edges.First();
        var p1 = _mesh.Point(edge.Node1);
        var p2 = _mesh.Point(edge.Node2);
        var v1 = _values[edge.Node1];
        var v2 = _values[edge.Node2];
        var interpolated = MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold);
        isoline.Start = interpolated;
        
        edge = _edges.Last();                                                  
        p1 = _mesh.Point(edge.Node1);                                          
        p2 = _mesh.Point(edge.Node2);                                          
        v1 = _values[edge.Node1];                                               
        v2 = _values[edge.Node2];                                               
        interpolated = MathHelper.InterpolateByValue(p1, p2, v1, v2, threshold);
        isoline.End = interpolated;

        return isoline;
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
                var element = _mesh.Element(j);
                var state = GetElementState(element);
                var isoline = MakeIsolineByState(state, threshold);

                if (isoline == null) continue;

                Isolines.Add(isoline.Value);
            }
        }
    }

    public void WriteIsolines()
    {
        var sw = new StreamWriter("C:/Users/lexan/source/repos/Python/isolines.txt");

        foreach (var i in Isolines)
        {
            sw.WriteLine($"{i.Start.X} {i.Start.Y} {i.End.X} {i.End.Y}", CultureInfo.InvariantCulture);
        }
        
        sw.Close();
    }
}