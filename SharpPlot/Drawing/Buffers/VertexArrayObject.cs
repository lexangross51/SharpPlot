using System;
using OpenTK.Graphics.OpenGL4;

namespace SharpPlot.Drawing.Buffers;

public class VertexArrayObject : IDisposable
{
    private readonly int _handle;
    private bool _isDisposed;

    public VertexArrayObject()
    {
        _handle = GL.GenVertexArray();
        GL.BindVertexArray(_handle);
    }

    public void SetAttributePointer(
        int location,
        int size,
        VertexAttribPointerType type,
        bool normalize,
        int stride,
        int offset
    )
    {
        GL.EnableVertexAttribArray(location);
        GL.VertexAttribPointer(location, size, type, normalize, stride, offset);
    }
    
    public void Bind() => GL.BindVertexArray(_handle);
    
    public void Unbind() => GL.BindVertexArray(0);
    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            GL.DeleteVertexArray(_handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}