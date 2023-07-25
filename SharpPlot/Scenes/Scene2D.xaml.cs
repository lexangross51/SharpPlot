using System;
using System.Drawing;
using System.Windows;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Wrappers;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly ShaderProgram _shaderProgram;
    private readonly VertexArrayObject _vao;
    private readonly VertexBufferObject<float> _vbo;
    private readonly ElementBufferObject _ebo;
    private readonly float[] _vertices =
    {
        0.5f, 0.5f, 0.0f,
        0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f, 0.5f, 0.0f
    };
    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    public Scene2D()
    {
        InitializeComponent();
        
        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });

        _shaderProgram = new ShaderProgram("Shaders/VertexShader.vert", "Shaders/FragmentShader.frag");
        _vbo = new VertexBufferObject<float>(_vertices);
        _vao = new VertexArrayObject();
        _vao.Bind();
        _vao.SetAttributePointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _ebo = new ElementBufferObject(_indices);
        
        GL.ClearColor(Color.Aqua);
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _shaderProgram.Use();
        _vao.Bind();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}