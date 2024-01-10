using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace SharpPlot.Core.Drawing.Buffers;

public class VertexBufferObject<T> : IDisposable where T : struct
{
    private readonly int _handle;
    private bool _isDisposed;

    public VertexBufferObject(T[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw)
    {
        _handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Marshal.SizeOf<T>(), data, hint);
    }

    public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);

    public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    public void UpdateData(T[] data)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, data.Length * Marshal.SizeOf<T>(), data);
    }
    
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