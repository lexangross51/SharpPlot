using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace SharpPlot.Core.Drawing.Image;

public class Texture : IDisposable
{
    private readonly int _handle;
    private static Rectangle _rectangle;
    private bool _isDisposed;

    static Texture()
    {
        _rectangle = new Rectangle();
    }

    private Texture()
    {
        _handle = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public Texture(Bitmap image) : this()
    {
        _rectangle.Height = image.Height;
        _rectangle.Width = image.Width;
        
        var data = image.LockBits(_rectangle,ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
        );
        
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            data.Width,
            data.Height,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            data.Scan0
        );
        
        image.UnlockBits(data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
    }
    
    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_isDisposed || !disposing) return;
        GL.DeleteTexture(_handle);
        _isDisposed = true;
    }
}