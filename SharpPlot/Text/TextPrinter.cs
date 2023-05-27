using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using SharpPlot.Render;

namespace SharpPlot.Text;

public enum TextOrientation : byte
{
    Horizontal,
    Vertical
}

public static class TextPrinter
{
    public static Size TextMeasure(string text, SharpPlotFont font)
        => TextRenderer.MeasureText(text, font.MakeFont());

    public static void DrawText(IBaseGraphic graphic, Caption text, double xLeft, double yBottom, TextOrientation orientation = TextOrientation.Horizontal)
    {
        var textSize = text.Size;
        Bitmap textImage = new(textSize.Width, textSize.Height);
        
        // Build texture
        using (var graphics = Graphics.FromImage(textImage))
        {
            graphics.Clear(Color.White);
            graphics.DrawString(text.Text, text.Font.MakeFont(), new SolidBrush(text.Font.Color), new PointF(0, 0));

            if (orientation == TextOrientation.Vertical)
            {
                textImage.RotateFlip(RotateFlipType.Rotate90FlipX);
            }
        }

        int textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureId);
        BitmapData data = textImage.LockBits(
            new Rectangle(0, 0, textImage.Width, textImage.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb
        );

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            data.Width,
            data.Height,
            0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
            PixelType.UnsignedByte,
            data.Scan0
        );

        textImage.UnlockBits(data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Render texture
        double w = textSize.Width / graphic.ScreenSize.Width * graphic.Projection.Width;
        double h = textSize.Height /*/ graphic.ScreenSize.Height * graphic.Projection.Height*/;

        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
        GL.Color3(Color.White);
        GL.Begin(PrimitiveType.Quads);

        if (orientation == TextOrientation.Horizontal)
        {
            GL.TexCoord2(0, 1); GL.Vertex2(xLeft, yBottom);
            GL.TexCoord2(1, 1); GL.Vertex2(xLeft + w, yBottom);
            GL.TexCoord2(1, 0); GL.Vertex2(xLeft + w, yBottom + h);
            GL.TexCoord2(0, 0); GL.Vertex2(xLeft, yBottom + h);
        }
        else
        {
            GL.TexCoord2(0, 0); GL.Vertex2(xLeft, yBottom);
            GL.TexCoord2(1, 0); GL.Vertex2(xLeft + w, yBottom);
            GL.TexCoord2(1, 1); GL.Vertex2(xLeft + w, yBottom + h);
            GL.TexCoord2(0, 1); GL.Vertex2(xLeft, yBottom + h);
        }

        GL.End();
        GL.Disable(EnableCap.Texture2D);
        textImage.Dispose();
    }
}