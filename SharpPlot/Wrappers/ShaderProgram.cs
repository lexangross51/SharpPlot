﻿using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace SharpPlot.Wrappers;

public sealed class ShaderProgram : IDisposable
{
    private readonly int _shaderProgram;
    private bool _isDisposed;

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
    }
    
    ~ShaderProgram()
    {
        if (_isDisposed) return;
        Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
    }

    public void Use() => GL.UseProgram(_shaderProgram);
    
    public int GetUniformLocation(string name) => GL.GetUniformLocation(_shaderProgram, name);

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
        if (_isDisposed) return;
        
        GL.DeleteProgram(_shaderProgram);
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}