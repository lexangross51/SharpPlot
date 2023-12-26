using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SharpPlot.Shaders;

public sealed class ShaderProgram : IDisposable
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniforms;
    private bool _isDisposed;

    public ShaderProgram(string vertexShaderPath, string fragmentShaderPath, string? geometryShaderPath)
    {
        _handle = GL.CreateProgram();
        
        var vertexShaderSource = File.ReadAllText(vertexShaderPath);
        var fragmentShaderSource = File.ReadAllText(fragmentShaderPath);
        var geometryShaderSource = geometryShaderPath != null 
            ? File.ReadAllText(geometryShaderPath) 
            : null;

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
        int? geometryShader = geometryShaderSource != null
            ? CompileShader(ShaderType.GeometryShader, geometryShaderSource)
            : null;
        
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        
        if (geometryShader != null)
        {
            GL.AttachShader(_handle, geometryShader.Value);
        }
        
        GL.LinkProgram(_handle);
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var status);

        if (status != 1)
        {
            throw new Exception($"Program link error: {GL.GetProgramInfoLog(_handle)}");
        }

        _uniforms = new Dictionary<string, int>();
        
        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var uniformsCount);

        for (int i = 0; i < uniformsCount; i++)
        {
            string name = GL.GetActiveUniform(_handle, i, out _, out _);
            int location = GL.GetUniformLocation(_handle, name);
            
            _uniforms.Add(name, location);
        }
    }

    ~ShaderProgram()
    {
        if (_isDisposed) return;
        Console.WriteLine("GPU resources leak! Did you forget to call Dispose()");
    }
    
    private int CompileShader(ShaderType shaderType, string shaderSource)
    {
        int id = GL.CreateShader(shaderType);
        GL.ShaderSource(id, shaderSource);
        GL.CompileShader(id);
        GL.GetShader(id, ShaderParameter.CompileStatus, out var status);

        if (status == (int)All.True) return id;

        throw new Exception($"Shader compile error: {GL.GetShaderInfoLog(id)}");
    }

    public void Use() => GL.UseProgram(_handle);

    // 1D uniforms
    public void SetUniform(string name, int value)
        => GL.Uniform1(_uniforms[name], value);

    public void SetUniform(string name, uint value)
        => GL.Uniform1(_uniforms[name], value);
    
    public void SetUniform(string name, float value)
        => GL.Uniform1(_uniforms[name], value);

    public void SetUniform(string name, double value)
        => GL.Uniform1(_uniforms[name], value);
    
    // 2D uniforms
    public void SetUniform(string name, float x, float y)
        => GL.Uniform2(_uniforms[name], x, y);

    public void SetUniforms(string name, double x, double y)
        => GL.Uniform2(_uniforms[name], x, y);

    public void SetUniform(string name, Vector2 vector)
        => GL.Uniform2(_uniforms[name], vector);
    
    // 3D uniforms
    public void SetUniform(string name, float x, float y, float z)
        => GL.Uniform3(_uniforms[name], x, y, z);

    public void SetUniforms(string name, double x, double y, double z)
        => GL.Uniform3(_uniforms[name], x, y, z);

    public void SetUniform(string name, Vector3 vector)
        => GL.Uniform3(_uniforms[name], vector);
    
    // 4D uniforms
    public void SetUniform(string name, float x, float y, float z, float w)
        => GL.Uniform4(_uniforms[name], x, y, z, w);

    public void SetUniform(string name, double x, double y, double z, double w)
        => GL.Uniform4(_uniforms[name], x, y, z, w);

    public void SetUniform(string name, Color4 color)
        => GL.Uniform4(_uniforms[name], color);
    
    public void SetUniform(string name, Quaternion quaternion)
        => GL.Uniform4(_uniforms[name], quaternion);
    
    public void SetUniform(string name, Vector4 vector)
        => GL.Uniform4(_uniforms[name], vector);
    
    // IDisposable implementation
    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        
        if (disposing)
        {
            GL.DeleteProgram(_handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}