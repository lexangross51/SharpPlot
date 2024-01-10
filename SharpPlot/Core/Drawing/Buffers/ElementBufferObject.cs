using System;
using OpenTK.Graphics.OpenGL4;

namespace SharpPlot.Core.Drawing.Buffers;

public class ElementBufferObject : IDisposable
{
    private readonly int _handle;
    private bool _isDisposed;

    public ElementBufferObject(uint[] indices, BufferUsageHint hint = BufferUsageHint.StaticDraw)
    {
        _handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _handle);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, hint);
    }

    public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, _handle);

    public void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    
    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            GL.DeleteBuffer(_handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}