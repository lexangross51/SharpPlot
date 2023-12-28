using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SharpPlot.Drawing.Shaders;

public sealed class ShaderProgram : IDisposable
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniforms;
    private bool _isDisposed;

    public ShaderProgram(string vertexShaderPath, string fragmentShaderPath, string? geometryShaderPath = null)
    {
        _handle = GL.CreateProgram();
        
        string vertexShaderSource = File.ReadAllText(vertexShaderPath);
        string fragmentShaderSource = File.ReadAllText(fragmentShaderPath);
        string? geometryShaderSource = geometryShaderPath != null 
            ? File.ReadAllText(geometryShaderPath) 
            : null;

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
        int geometryShader = geometryShaderSource != null
            ? CompileShader(ShaderType.GeometryShader, geometryShaderSource)
            : -1;
        
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        
        if (geometryShader != -1)
        {
            GL.AttachShader(_handle, geometryShader);
        }
        
        GL.LinkProgram(_handle);
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var status);

        if (status != 1)
        {
            throw new Exception($"Program link error: {GL.GetProgramInfoLog(_handle)}");
        }
        
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        
        if (geometryShader != -1)
        {
            GL.DetachShader(_handle, geometryShader);
        }
        
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        if (geometryShader != -1)
        {
            GL.DeleteShader(geometryShader);
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
    
    private static int CompileShader(ShaderType shaderType, string shaderSource)
    {
        int id = GL.CreateShader(shaderType);
        GL.ShaderSource(id, shaderSource);
        GL.CompileShader(id);
        GL.GetShader(id, ShaderParameter.CompileStatus, out var status);

        if (status == (int)All.True) return id;

        throw new Exception($"Shader compile error: {GL.GetShaderInfoLog(id)}");
    }

    public void Use() => GL.UseProgram(_handle);

    // Attributes
    public void GetAttributeLocation(string name, out int location)
        => location = GL.GetAttribLocation(_handle, name);
    
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

    public void SetUniform(string name, double x, double y)
        => GL.Uniform2(_uniforms[name], x, y);

    public void SetUniform(string name, Vector2 vector)
        => GL.Uniform2(_uniforms[name], vector);
    
    // 3D uniforms
    public void SetUniform(string name, float x, float y, float z)
        => GL.Uniform3(_uniforms[name], x, y, z);

    public void SetUniform(string name, double x, double y, double z)
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

    public void SetUniform(string name, Matrix4 matrix)
        => GL.UniformMatrix4(_uniforms[name], true, ref matrix);
    
    // IDisposable implementation
    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        
        if (disposing)
        {
            GL.DeleteProgram(_handle);
            _uniforms.Clear();
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}