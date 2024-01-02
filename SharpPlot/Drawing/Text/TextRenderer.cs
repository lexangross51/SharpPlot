using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Drawing.Buffers;
using SharpPlot.Drawing.Image;
using SharpPlot.Drawing.Render;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Shaders;

namespace SharpPlot.Drawing.Text;

public class TextRenderer
{
    private static TextRenderer? _renderer;
    private static Font? _font;
    private readonly SolidBrush _brush;
    private readonly PointF _startPoint;
    private static readonly float[] TextPosition =
    [
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
        0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f
    ];
    private static readonly uint[] Indices =
    [
        0, 1, 3,
        1, 2, 3
    ];

    private Matrix4 _projectionMatrix;
    private RenderSettings _settings;
    private readonly ShaderProgram _shader;
    private readonly VertexArrayObject _vao;
    private readonly VertexBufferObject<float> _vbo;

    public IProjection Projection { get; set; } = null!;

    public RenderSettings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(
                (float)_settings.Margin, (float)(_settings.ScreenWidth - _settings.Margin),
                (float)_settings.Margin, (float)(_settings.ScreenHeight - _settings.Margin), 
                -1.0f, 1.0f);
        }
    }

    private TextRenderer()
    {
        _brush = new SolidBrush(Color.Black);
        _startPoint = new PointF(0.0f, 0.0f);
        
        _shader = new ShaderProgram(
            "Drawing/Shaders/Sources/Text/TextShader.vert",
            "Drawing/Shaders/Sources/Text/TextShader.frag");
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(TextPosition);
        _ = new ElementBufferObject(Indices);
        
        _shader.Use();
        _shader.SetUniform("projection", Matrix4.Identity);
        _shader.GetAttributeLocation("position", out var position);
        _shader.GetAttributeLocation("texPosition", out var texPosition);
        _vao.SetAttributePointer(position, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        _vao.SetAttributePointer(texPosition, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
    }

    public static TextRenderer GetInstance() => _renderer ??= new TextRenderer();
    
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
        
        using var graphics = Graphics.FromImage(textImage);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.DrawString(text, _font, _brush, _startPoint);

        if (orientation == TextRenderOrientation.Vertical)
        {
            textImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }

        int w = textImage.Width;
        int h = textImage.Height;
        var texture = new Texture(textImage);
        textImage.Dispose();
        
        var proj = Projection.ToArray();
        double spx = Settings.Margin + (x - proj[0]) / (proj[1] - proj[0]) * (Settings.ScreenWidth - Settings.Margin);
        double spy = Settings.Margin + (y - proj[2]) / (proj[3] - proj[2]) * (Settings.ScreenHeight - Settings.Margin);
        
        TextPosition[0] = (float)spx;
        TextPosition[1] = (float)spy;
        TextPosition[5] = (float)(spx + w);
        TextPosition[6] = (float)spy;
        TextPosition[10] = (float)(spx + w);
        TextPosition[11] = (float)(spy + h);
        TextPosition[15] = (float)spx;
        TextPosition[16] = (float)(spy + h);
        
        _vbo.Bind();
        _vbo.UpdateData(TextPosition);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        _vao.Bind();
        
        texture.Use();
        _shader.Use();
        _shader.SetUniform("projection", _projectionMatrix);
        
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        texture.Dispose();
    }
}