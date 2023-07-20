using System;
using System.Windows;
using SharpPlot.Wrappers;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly ShaderProgram _shaderProgram;
    
    public Scene2D()
    {
        InitializeComponent();
        _shaderProgram = new ShaderProgram("../Shaders/VertexShader.vert", "../Shaders/FragmentShader.frag");
    }

    private void OnRender(TimeSpan obj)
    {
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
    }
}