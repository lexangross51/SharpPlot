using System;
using System.Diagnostics;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Wrappers;
using Color = System.Drawing.Color;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly Camera.Camera _camera;
    private readonly ShaderProgram _shaderProgram;
    private readonly VertexArrayObject _vao;
    private readonly Texture.Texture _texture0;
    private readonly Texture.Texture _texture1;
    private readonly float[] _vertices =
    {
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f
    };

    private readonly Stopwatch _timer = new();
    private readonly DeltaTime _deltaTime = new();

    public Scene2D()
    {
        InitializeComponent();
        
        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 6,
            RenderContinuously = true,
        });

        _camera = new Camera.Camera(new Vector3(0, 0, 3))
        {
            AspectRation = 800.0f / 500.0f
        };
        
        _vao = new VertexArrayObject();
        _vao.Bind();

        var vbo = new VertexBufferObject<float>(_vertices);

        _shaderProgram = new ShaderProgram("Shaders/BaseShader.vert", "Shaders/BaseShader.frag");
        _shaderProgram.Use();
        
        _shaderProgram.GetAttribLocation("position", out int posLocation);
        _shaderProgram.GetAttribLocation("inTexCoord", out int texLocation);
        
        _vao.SetAttributePointer(posLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        _vao.SetAttributePointer(texLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        _texture0 = new Texture.Texture("container.jpg");
        _texture0.Use();
        _texture1 = new Texture.Texture("wall.jpg");
        _texture1.Use(TextureUnit.Texture1);
        
        _shaderProgram.SetUniform("texture0", 0);
        _shaderProgram.SetUniform("texture1", 1);

        _timer.Start();
     
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(Color.White);
    }

    private void OnRender(TimeSpan obj)
    {
        float deltaTime = (float)obj.TotalMilliseconds;
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        
        _vao.Bind();
        _texture0.Use();
        _texture1.Use(TextureUnit.Texture1);

        var model = Matrix4.CreateRotationY((float)(Math.Sin(2 * _timer.Elapsed.TotalSeconds) + Math.Cos(2 * _timer.Elapsed.TotalSeconds)));
        model *= Matrix4.CreateRotationX((float)Math.Cos(2 * _timer.Elapsed.TotalSeconds));
        model *= Matrix4.CreateRotationZ((float)_timer.Elapsed.TotalSeconds * 2);
        
        _camera.Speed = 20f * _deltaTime.Result();
        
        _shaderProgram.Use();
        _shaderProgram.SetUniform("model", model);
        _shaderProgram.SetUniform("view", _camera.ViewMatrix());
        _shaderProgram.SetUniform("projection", _camera.ProjectionMatrix());
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        if (e.KeyboardDevice.IsKeyDown(Key.W)) _camera.Move(CameraDirection.Forward);
        if (e.KeyboardDevice.IsKeyDown(Key.S)) _camera.Move(CameraDirection.Backward);
        if (e.KeyboardDevice.IsKeyDown(Key.A)) _camera.Move(CameraDirection.Left);
        if (e.KeyboardDevice.IsKeyDown(Key.D)) _camera.Move(CameraDirection.Right);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var x = (float)e.GetPosition(this).X;
            var y = (float)e.GetPosition(this).Y;
        
            _camera.LookAt(x, y);
        }
        else
        {
            _camera.IsFirstMove = true;
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta < 0)
            _camera.FieldOfView += 1.0f;
        else
            _camera.FieldOfView -= 1.0f;
    }
}