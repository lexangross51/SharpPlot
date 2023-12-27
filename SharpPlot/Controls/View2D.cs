using System;
using System.Windows;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Buffers;
using SharpPlot.Shaders;

namespace SharpPlot.Controls;

public class View2D : GLWpfControl
{
    private ShaderProgram? _shader;
    private VertexArrayObject _vao = null!;
    
    public View2D()
    {
        Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });
        
        Loaded += OnLoaded;
        Render += RenderScene;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _shader = new ShaderProgram("Shaders/Sources/AxesShader.vert",
            "Shaders/Sources/AxesShader.frag",
            "Shaders/Sources/AxesShader.geom");
        
        _vao = new VertexArrayObject();
        _shader.Use();
        _shader.GetAttributeLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _shader.SetUniform("projection", Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1));
        _shader.SetUniform("startX", -1.0f);
        _shader.SetUniform("endX", 1.0f);
        _shader.SetUniform("stepX", 0.5f);
        _shader.SetUniform("screenSize", (float)Width, (float)Height);
        _shader.SetUniform("marginPixels", 30);
        
        _vao.Unbind();
    }

    private void RenderScene(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        if (_shader == null)
            return;
        
        _shader.Use();
        _vao.Bind();
        
        GL.DrawArrays(PrimitiveType.Points, 0, 1);
    }
}