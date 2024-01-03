using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL4;
using SharpPlot.Drawing.Buffers;
using SharpPlot.Drawing.Image;
using SharpPlot.Drawing.Render;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Shaders;

namespace SharpPlot.Drawing.Text;

public class TextRenderer
{
    private static TextRenderer? _renderer;
    private Font? _font;
    private readonly SolidBrush _brush;
    private readonly PointF _startPoint;
    private readonly float[] _textPosition =
    [
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
        0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f
    ];

    private readonly ShaderProgram _shader;
    private readonly VertexArrayObject _vao;
    private readonly VertexBufferObject<float> _vbo;

    public IProjection Projection { get; set; } = null!;
    public FrameSettings Settings { get; set; } = null!;

    public static TextRenderer Instance => _renderer ??= new TextRenderer();

    private TextRenderer()
    {
        _brush = new SolidBrush(Color.Black);
        _startPoint = new PointF(0.0f, 0.0f);
        
        _shader = new ShaderProgram(
            "Drawing/Shaders/Sources/Text/TextShader.vert",
            "Drawing/Shaders/Sources/Text/TextShader.frag");
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(_textPosition);
        
        _shader.Use();
        _shader.GetAttributeLocation("position", out var position);
        _shader.GetAttributeLocation("texPosition", out var texPosition);
        _vao.SetAttributePointer(position, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        _vao.SetAttributePointer(texPosition, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
    }
    
    public static void TextMeasure(string text, SharpPlotFont font, out double width, out double height)
    {
        var measureText = System.Windows.Forms.TextRenderer.MeasureText(text, font.SystemFont);
        width = measureText.Width;
        height = measureText.Height;
    }
    
    public void Print(double x, double y, double z, string text, SharpPlotFont font, Color color,
        TextRenderOrientation orientation = TextRenderOrientation.Horizontal)
    {
        _font = font.SystemFont;
        _brush.Color = color;
        
        TextMeasure(text, font, out var width, out var height);
        var textImage = new Bitmap((int)width, (int)height);
        
        using (var graphics = Graphics.FromImage(textImage))
        {
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.DrawString(text, _font, _brush, _startPoint);
        }

        if (orientation == TextRenderOrientation.Vertical)
        {
            textImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }
        
        var proj = Projection.ToArray();
        var w = textImage.Width / Settings.ScreenWidth * (proj[1] - proj[0]);
        var h = textImage.Height / Settings.ScreenHeight * (proj[3] - proj[2]);
        var texture = new Texture(textImage);
        
        textImage.Dispose();
        
        _textPosition[0] = (float)x;
        _textPosition[1] = (float)y;
        _textPosition[5] = (float)(x + w);
        _textPosition[6] = (float)y;
        _textPosition[10] = (float)(x + w);
        _textPosition[11] = (float)(y + h);
        _textPosition[15] = (float)x;
        _textPosition[16] = (float)(y + h);
        
        _vbo.Bind();
        _vbo.UpdateData(_textPosition);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        _vao.Bind();
        
        texture.Use();
        _shader.Use();
        _shader.SetUniform("projection", Projection.ProjectionMatrix);
        
        GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        GL.Disable(EnableCap.Blend);
        
        texture.Dispose();
    }
}