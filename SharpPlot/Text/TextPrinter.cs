﻿using System.Drawing;
using System.Drawing.Imaging;
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

        BitmapData data = textImage.LockBits(
            new Rectangle(0, 0, textImage.Width, textImage.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );

        // Render texture
        double w = textSize.Width / graphic.ScreenSize.Width * graphic.Projection.Width;
        double h = textSize.Height /*/ graphic.ScreenSize.Height * graphic.Projection.Height*/;

        _texture ??= new Texture();
        _texture.Create(graphic.GL);
        graphic.GL.BindTexture(OpenGL.GL_TEXTURE_2D, _texture.TextureName);
        graphic.GL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 
            OpenGL.GL_RGB, 
            textImage.Width, textImage.Height, 
            0, 32993u, 5121u, data.Scan0);
        textImage.UnlockBits(data);
        textImage.Dispose();


        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
        graphic.GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

        graphic.GL.Enable(OpenGL.GL_TEXTURE_2D);
        graphic.GL.Color(1f, 1f, 1f);
        graphic.GL.Begin(OpenGL.GL_QUADS);

        if (orientation == TextOrientation.Horizontal)
        {
            graphic.GL.TexCoord(0, 1); graphic.GL.Vertex(xLeft, yBottom);
            graphic.GL.TexCoord(1, 1); graphic.GL.Vertex(xLeft + w, yBottom);
            graphic.GL.TexCoord(1, 0); graphic.GL.Vertex(xLeft + w, yBottom + h);
            graphic.GL.TexCoord(0, 0); graphic.GL.Vertex(xLeft, yBottom + h);
        }
        else
        {
            graphic.GL.TexCoord(0, 0); graphic.GL.Vertex(xLeft, yBottom);
            graphic.GL.TexCoord(1, 0); graphic.GL.Vertex(xLeft + w, yBottom);
            graphic.GL.TexCoord(1, 1); graphic.GL.Vertex(xLeft + w, yBottom + h);
            graphic.GL.TexCoord(0, 1); graphic.GL.Vertex(xLeft, yBottom + h);
        }

        graphic.GL.End();
        graphic.GL.Disable(OpenGL.GL_TEXTURE_2D);
    }
}