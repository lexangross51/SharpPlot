using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Shaders;
using SharpPlot.Wrappers;

namespace SharpPlot.Core.Colorbar;

public partial class Colorbar
{
    private readonly ShaderProgram _shaderBorders;
    private readonly ShaderProgram _shaderQuads;

    private const int MaxValuesCount = 8;    
    private readonly double[] _values;
    private readonly Palette.Palette _palette;
    private readonly ColorInterpolationType _interpolationType;
    
    public Colorbar(IEnumerable<double> values, Palette.Palette palette, ColorInterpolationType interpolationType = ColorInterpolationType.Linear)
    {
        InitializeComponent();
        
        CbControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });
        
        Width = 20;
        Height = 40;

        _shaderBorders = ShaderCollection.LineShader();
        _shaderQuads = ShaderCollection.FieldShader();

        _values = values.ToArray();
        _palette = palette;
        _interpolationType = interpolationType;
        
        MakeColoredQuads();
        
        GL.ClearColor(Color.White);
    }

    private void MakeColoredQuads()
    {
        double max = _values.Max();
        double min = _values.Min();
        int colorsCount = _palette.ColorsCount;

        var data = new float[6 * (colorsCount + 1)];
        var xLeft = -0.8f;
        var xRight = 0.0f;
        var yBottom = -0.8f;
        var yTop = 0.8f;

        if (_interpolationType == ColorInterpolationType.Nearest)
        {
            int index = 0;
            data[index++] = xLeft;
            data[index++] = yBottom;
            data[index++] = 0.0f;
            data[index++] = _palette[0].R;
            data[index++] = _palette[0].G;
            data[index++] = _palette[0].B;

            // foreach (var VARIABLE in COLLECTION)
            // {
            //     
            // }
        }
        else
        {
            
        }
    }
    
    private void OnRender(TimeSpan obj)
    {
        
    }
}