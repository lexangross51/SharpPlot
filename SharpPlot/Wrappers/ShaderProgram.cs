using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SharpPlot.Wrappers;

public sealed class ShaderProgram : IDisposable
{
    private readonly int _shaderProgram;
    private bool _isDisposed;
    private readonly Dictionary<string, int> _uniforms;

    public ShaderProgram(string vertexShaderPath, string fragmentShaderPath)
    {
        var vertexShaderSource = File.ReadAllText(vertexShaderPath);
        var fragmentShaderSource = File.ReadAllText(fragmentShaderPath);
        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        _shaderProgram = GL.CreateProgram();
        
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);
        GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out var status);

        if (status != 1) 
            throw new Exception($"Program link error: {GL.GetProgramInfoLog(_shaderProgram)}");
        
        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // All active uniforms
        _uniforms = new Dictionary<string, int>();
        GL.GetProgram(_shaderProgram, GetProgramParameterName.ActiveUniforms, out var uniformsCount);

        for (int i = 0; i < uniformsCount; i++)
        {
            var name = GL.GetActiveUniform(_shaderProgram, i, out _, out _);
            var location = GL.GetUniformLocation(_shaderProgram, name);
            _uniforms.Add(name, location);
        }
    }
    
    ~ShaderProgram()
    {
        if (_isDisposed) return;
        Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
    }

    public void Use() => GL.UseProgram(_shaderProgram);

    public void GetAttribLocation(string name, out int location)
        => location = GL.GetAttribLocation(_shaderProgram, name);
    
    public void GetUniformLocation(string name, out int location) 
        => location = _uniforms[name];

    public void SetUniform(int location, float r, float g, float b, float a) 
        => GL.Uniform4(location, r, g, b, a);

    public void SetUniform(string name, int value)
        => GL.Uniform1(_uniforms[name], value);
    
    public void SetUniform(string name, float value)
        => GL.Uniform1(_uniforms[name], value);
    
    public void SetUniform(string name, float[] array)
        => GL.Uniform4(_uniforms[name], array.Length / 4, array);
    
    public void SetUniform(string name, Matrix4 matrix)
        => GL.UniformMatrix4(_uniforms[name], true, ref matrix);
    
    private int CompileShader(ShaderType shaderType, string shaderSource)
    {
        int id = GL.CreateShader(shaderType);
        GL.ShaderSource(id, shaderSource);
        GL.CompileShader(id);
        GL.GetShader(id, ShaderParameter.CompileStatus, out var status);

        if (status == (int)All.True) return id;

        throw new Exception($"Shader compile error: {GL.GetShaderInfoLog(id)}");
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed || !disposing) return;
        
        GL.DeleteProgram(_shaderProgram);
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}