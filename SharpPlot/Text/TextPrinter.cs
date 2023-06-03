using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpPlot.Render;

namespace SharpPlot.Text;

public enum TextOrientation : byte
{
    Horizontal,
    Vertical
}

public static class TextPrinter
{
    private static Texture? _texture;

    public static Size TextMeasure(string text, SharpPlotFont font)
        => TextRenderer.MeasureText(text, font.MakeFont());

    public static void DrawText(IBaseGraphic graphic, string text, double x, double y, SharpPlotFont font, 
        TextOrientation orientation = TextOrientation.Horizontal)
    {
        var textSize = TextMeasure(text, font);
        Bitmap textImage = new(textSize.Width, textSize.Height);

        // Build texture
        using (var graphics = Graphics.FromImage(textImage))
        {
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.DrawString(text, font.MakeFont(), new SolidBrush(font.Color), new PointF(0, 0));

            if (orientation == TextOrientation.Vertical)
            {
                textImage.RotateFlip(RotateFlipType.Rotate90FlipX);
            }
        }

        double w = textImage.Width / graphic.ScreenSize.Width * graphic.Projection.Width;
        double h = textImage.Height / graphic.ScreenSize.Height * graphic.Projection.Height;

        BitmapData data = textImage.LockBits(
            new Rectangle(0, 0, textImage.Width, textImage.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );

        // Render texture
        _texture ??= new Texture();
        _texture.Create(graphic.GL);
        _texture.Bind(graphic.GL);
        graphic.GL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0,
            OpenGL.GL_RGBA,
            textImage.Width, textImage.Height,
            0, 32993u, 5121u, data.Scan0);
        textImage.UnlockBits(data);
        textImage.Dispose();

        graphic.GL.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);
        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_AUTO_GENERATE_MIPMAP, OpenGL.GL_TRUE);

        graphic.GL.Enable(OpenGL.GL_BLEND);
        graphic.GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        graphic.GL.Enable(OpenGL.GL_TEXTURE_2D);
        graphic.GL.Color(1f, 1f, 1f, 1f);
        graphic.GL.Begin(OpenGL.GL_QUADS);

        if (orientation == TextOrientation.Horizontal)
        {
            graphic.GL.TexCoord(0, 1);
            graphic.GL.Vertex(x, y);
            graphic.GL.TexCoord(1, 1);
            graphic.GL.Vertex(x + w, y);
            graphic.GL.TexCoord(1, 0);
            graphic.GL.Vertex(x + w, y + h);
            graphic.GL.TexCoord(0, 0);
            graphic.GL.Vertex(x, y + h);
        }
        else
        {
            graphic.GL.TexCoord(0, 0);
            graphic.GL.Vertex(x, y);
            graphic.GL.TexCoord(1, 0);
            graphic.GL.Vertex(x + w, y);
            graphic.GL.TexCoord(1, 1);
            graphic.GL.Vertex(x + w, y + h);
            graphic.GL.TexCoord(0, 1);
            graphic.GL.Vertex(x, y + h);
        }

        graphic.GL.End();
        graphic.GL.Disable(OpenGL.GL_TEXTURE_2D);
    }
}