using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SharpPlot.Geometry;

namespace SharpPlot.Helpers;

public static class Utilities
{
    public static void ReadData(string filename, out List<Point3D> points, out List<double> values)
    {
        points = new List<Point3D>();
        values = new List<double>();
        
        var lines = File.ReadAllLines(filename);

        foreach (var line in lines)
        {
            var lLine = line.Replace(',', '.');
            var words = lLine.Split().Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point3D
            {
                X = words[0],
                Y = words[1],
            });
            values.Add(words[2]);
        }
    }
    
    public static void ReadData(string filename, out List<Point3D> points)
    {
        points = new List<Point3D>();
        var lines = File.ReadAllLines(filename);

        foreach (var line in lines)
        {
            var lLine = line.Replace(',', '.');
            var words = lLine.Split().Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point3D
            {
                X = words[0],
                Y = words[1],
            });
        }
    }
    
    public static void ReadData(string fPoints, string fValues, out List<Point3D> points, out List<double> values)
    {
        points = new List<Point3D>();
        values = new List<double>();
        
        var lines = File.ReadAllLines(fPoints);
        foreach (var line in lines)
        {
            var lLine = line.Replace(',', '.');
            var words = lLine.Split().Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point3D
            {
                X = words[0],
                Y = words[1],
            });
        }
        
        lines = File.ReadAllLines(fValues);
        foreach (var line in lines)
        {
            var words = line.Split(' ', '\t', '\n');

            foreach (var word in words)
            {
                if (word == "") continue;
                var wWord = word.Replace(',', '.');
                values.Add(double.Parse(wWord, CultureInfo.InvariantCulture));
            }
        }
    }
}