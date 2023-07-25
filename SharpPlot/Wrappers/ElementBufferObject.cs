using System;
using OpenTK.Graphics.OpenGL4;

namespace SharpPlot.Wrappers;

public class ElementBufferObject : IBindable, IDisposable
{
    private readonly int _handle;

    public ElementBufferObject(uint[] indices, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
    {
        _handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _handle);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, usageHint);
    }

    public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, _handle);

    public void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

    public void Dispose() => GL.DeleteBuffer(_handle);
}