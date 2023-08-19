// using SharpPlot.Viewport;
// using SharpPlot.Wrappers;
//
// namespace SharpPlot.Render;
//
// public class BaseGraphic2D : IBaseGraphic
// {
//     public ShaderProgram Shader { get; }
//     public ScreenSize ScreenSize { get; set; }
//     public Indent Indent { get; set; }
//     public IProjection Projection { get; set; }
//     
//     public BaseGraphic2D(ScreenSize screenSize, IProjection projection, Indent indent = new())
//     {
//         ScreenSize = screenSize;
//         Projection = projection;
//         Indent = indent;
//     }
//     
//     public double[] GetNewViewPort(ScreenSize newScreenSize)
//     {
//         ScreenSize = new ScreenSize
//         {
//             Width = newScreenSize.Width - Indent.Horizontal - 2, 
//             Height = newScreenSize.Height - Indent.Vertical - 2
//         };
//
//         return new[]
//         {
//             Indent.Horizontal,
//             Indent.Vertical,
//             ScreenSize.Width,
//             ScreenSize.Height
//         };
//     }
//
//     public void UpdateView()
//     {
//         throw new System.NotImplementedException();
//     }
// }