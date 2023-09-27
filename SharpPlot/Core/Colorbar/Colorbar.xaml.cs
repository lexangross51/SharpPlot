using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Render;
using SharpPlot.Shaders;
using SharpPlot.Text;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;
using Brushes = System.Windows.Media.Brushes;

namespace SharpPlot.Core.Colorbar;

public partial class Colorbar
{
    private readonly Viewport2DRenderer _viewPortRenderer;
    private readonly ShaderProgram _shaderBorders;
    private readonly ShaderProgram _shaderQuads;
    private readonly uint[] _indices;
    private VertexArrayObject _vaoQuads = null!;
    private VertexArrayObject _vaoBorder = null!;
    private int _pointsCount;

    private const int MaxValuesCount = 8;
    private readonly double _minValue;
    private readonly double _maxValue;
    private readonly Palette.Palette _palette;
    private readonly ColorInterpolationType _interpolationType;

    private const float L = -0.8f;
    private const float R = -0.4f;
    private const float B = -0.9f;
    private const float T = 0.9f;
    
    public Colorbar(IEnumerable<double> values, Palette.Palette palette, ColorInterpolationType interpolationType = ColorInterpolationType.Linear)
    {
        InitializeComponent();

        Width = 120;
        Height = 200;
        
        CbControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });
        
        BorderBrush = Brushes.Black;
        BorderThickness = new Thickness(1);
        
        var renderSettings = new RenderSettings
        {
            ScreenSize = new ScreenSize
            {
                Width = Width,
                Height = Height,
            },
            Indent = new Indent()
        };

        var camera = new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, Height / Width));

        _viewPortRenderer = new Viewport2DRenderer(renderSettings, camera) { Font = new SharpPlotFont()};
        
        _shaderBorders = ShaderCollection.LineShader();
        _shaderQuads = ShaderCollection.FieldShader();

        var valuesArray = values.ToArray();
        _minValue = valuesArray.Min();
        _maxValue = valuesArray.Max();
        
        _palette = palette;
        _indices = new uint[_palette.ColorsCount * 4];
        _interpolationType = interpolationType;
        
        MakeColoredQuads();
        MakeBorderAndValuesTicks();
        
        GL.ClearColor(Color.White);
    }

    private void MakeColoredQuads()
    {
        int colorsCount = _palette.ColorsCount;
        var data = new float[6 * (colorsCount + 1) * 2];
        double h = (T - B) / colorsCount;

        int index = 0;
        data[index++] = L;
        data[index++] = B;
        data[index++] = 0.0f;
        data[index++] = _palette[^1].R;
        data[index++] = _palette[^1].G;
        data[index++] = _palette[^1].B;
        data[index++] = R;
        data[index++] = B;
        data[index++] = 0.0f;
        data[index++] = _palette[^1].R;
        data[index++] = _palette[^1].G;
        data[index++] = _palette[^1].B;
        
        for (int i = 1; i < colorsCount; i++)
        {
            data[index++] = L;
            data[index++] = (float)(B + i * h);
            data[index++] = 0.0f;
            data[index++] = (_palette[^i].R + _palette[^(i + 1)].R) / 2.0f;
            data[index++] = (_palette[^i].G + _palette[^(i + 1)].G) / 2.0f;
            data[index++] = (_palette[^i].B + _palette[^(i + 1)].B) / 2.0f;
            data[index++] = R;
            data[index++] = (float)(B + i * h);
            data[index++] = 0.0f;
            data[index++] = (_palette[^i].R + _palette[^(i + 1)].R) / 2.0f;
            data[index++] = (_palette[^i].G + _palette[^(i + 1)].G) / 2.0f;
            data[index++] = (_palette[^i].B + _palette[^(i + 1)].B) / 2.0f;
        }
        
        data[index++] = L;
        data[index++] = T;
        data[index++] = 0.0f;
        data[index++] = _palette[0].R;
        data[index++] = _palette[0].G;
        data[index++] = _palette[0].B;
        data[index++] = R;
        data[index++] = T;
        data[index++] = 0.0f;
        data[index++] = _palette[0].R;
        data[index++] = _palette[0].G;
        data[index] = _palette[0].B;

        index = 0;
        for (int i = 0; i < colorsCount; i++)
        {
            _indices[index++] = (uint)i * 2;
            _indices[index++] = (uint)i * 2 + 1;
            _indices[index++] = (uint)i * 2 + 3;
            _indices[index++] = (uint)i * 2 + 2;
        }
        
        _vaoQuads = new VertexArrayObject();
        var vbo = new VertexBufferObject<float>(data);
            
        _shaderQuads.Use();
        _shaderQuads.GetAttribLocation("position", out var location);
        _vaoQuads.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        _shaderQuads.GetAttribLocation("color", out location);
        _vaoQuads.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            
        _ = new ElementBufferObject(_indices);
            
        vbo.Unbind();
        _vaoQuads.Unbind();
    }

    private void MakeBorderAndValuesTicks()
    {
        int colorsCount = _palette.ColorsCount;
        var (valueStep, valuesCount) = colorsCount < MaxValuesCount - 1
            ? ((_maxValue - _minValue) / colorsCount, colorsCount + 1)
            : ((_maxValue - _minValue) / (MaxValuesCount - 1), MaxValuesCount);

        var data = new float[24 + valuesCount * 6];
        _pointsCount = 8 + valuesCount * 2;
        int index = 0;

        data[index++] = L;
        data[index++] = B;
        data[index++] = 0.0f;
        data[index++] = R;
        data[index++] = B;
        data[index++] = 0.0f;
        
        data[index++] = R;
        data[index++] = B;
        data[index++] = 0.0f;
        data[index++] = R;
        data[index++] = T;
        data[index++] = 0.0f;
        
        data[index++] = R;
        data[index++] = T;
        data[index++] = 0.0f;
        data[index++] = L;
        data[index++] = T;
        data[index++] = 0.0f;
        
        data[index++] = L;
        data[index++] = T;
        data[index++] = 0.0f;
        data[index++] = L;
        data[index++] = B;
        data[index++] = 0.0f;

        float h = (T - B) / colorsCount;
        
        for (int i = 0; i < valuesCount; i++)
        {
            data[index++] = R;
            data[index++] = B + i * h;
            data[index++] = 0.0f;
            
            data[index++] = R + 0.07f;
            data[index++] = B + i * h;
            data[index++] = 0.0f;
        }

        _vaoBorder = new VertexArrayObject();
        var vbo = new VertexBufferObject<float>(data);
        _shaderBorders.Use();
        _shaderBorders.GetUniformLocation("lineColor", out int location);
        _shaderBorders.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);
        _shaderQuads.GetAttribLocation("position", out location);
        _vaoBorder.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        
        vbo.Unbind();
        _vaoBorder.Unbind();
    }

    private void DrawLabels()
    {
        int colorsCount = _palette.ColorsCount;
        var (valueStep, valuesCount) = colorsCount < MaxValuesCount - 1
            ? ((_maxValue - _minValue) / colorsCount, colorsCount + 1)
            : ((_maxValue - _minValue) / (MaxValuesCount - 1), MaxValuesCount);

        float h = (T - B) / colorsCount;
        
        for (int i = 0; i < valuesCount; i++)
        {
            TextPrinter.DrawText(_viewPortRenderer, $"{double.Round(_minValue + i * valueStep, 4)}", 
                R + 0.1f, B + i * h - 0.1f, _viewPortRenderer.Font);
        }
    }
    
    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Colored quads
        _vaoQuads.Bind();
        _shaderQuads.Use();
        _shaderQuads.SetUniform("model", Matrix4.Identity);
        _shaderQuads.SetUniform("view", Matrix4.Identity);
        _shaderQuads.SetUniform("projection", Matrix4.Identity);
        
        GL.DrawElements(BeginMode.Quads, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
        _vaoQuads.Unbind();
        
        // Border and ticks
        _vaoBorder.Bind();
        _shaderBorders.Use();
        _shaderBorders.SetUniform("model", Matrix4.Identity);
        _shaderBorders.SetUniform("view", Matrix4.Identity);
        _shaderBorders.SetUniform("projection", Matrix4.Identity);
        
        GL.DrawArrays(PrimitiveType.Lines, 0, _pointsCount);
        
        _vaoBorder.Unbind();
        
        DrawLabels();
    }
}