// using System.Collections.Generic;
// using SharpGL.Enumerations;
// using SharpPlot.Objects;
//
// namespace SharpPlot.Render;
//
// public class Viewport3DRenderer : IRenderer
// {
//     public IBaseGraphic BaseGraphic { get; set; }
//     public List<IBaseObject> RenderableObjects { get; }
//
//     public Viewport3DRenderer(IBaseGraphic baseGraphic)
//     {
//         BaseGraphic = baseGraphic;
//         RenderableObjects = new List<IBaseObject>();
//     }
//
//     public void Draw()
//     {
//         RenderBorders();
//         RendererAxes();
//     }
//
//     private void RendererAxes()
//     {
//         var gl = BaseGraphic.GL;
//         
//         gl.MatrixMode(MatrixMode.Projection);
//         gl.PushMatrix();
//         gl.LoadIdentity();
//         gl.Viewport(0, 0, 80, 80);
//         gl.Ortho(-20.0, 20.0, -20.0, 20.0, -20.0, 20.0);
//         gl.MatrixMode(MatrixMode.Modelview);
//         gl.PushMatrix();
//         gl.LoadIdentity();
//         //gl.Rotate(-90.0, 1.0, 0.0, 0.0);
//         
//         // Axes
//         gl.Begin(BeginMode.Lines);
//         gl.Color(1f, 0f, 0f);
//         gl.Vertex(0, 0, 0);
//         gl.Vertex(10, 0, 0);
//         gl.Color(0f, 1f, 0f);
//         gl.Vertex(0, 0, 0);
//         gl.Vertex(0, 10, 0);
//         gl.Color(0f, 0f, 1f);
//         gl.Vertex(0, 0, 0);
//         gl.Vertex(0, 0, 10);
//         gl.End();
//
//         double sizeArrow = 1, sizeArrowAx = 7;
//         gl.Begin(BeginMode.Triangles);
//
//         gl.Color(1f, 0f, 0f);
//         gl.Vertex(10, 0, 0);
//         gl.Vertex(sizeArrowAx, -sizeArrow, -sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, -sizeArrow);
//
//         gl.Vertex(10, 0, 0);
//         gl.Vertex(sizeArrowAx, sizeArrow, -sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, sizeArrow);
//
//         gl.Vertex(10, 0, 0);
//         gl.Vertex(sizeArrowAx, -sizeArrow, sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, sizeArrow);
//
//         gl.Vertex(10, 0, 0);
//         gl.Vertex(sizeArrowAx, -sizeArrow, -sizeArrow);
//         gl.Vertex(sizeArrowAx, -sizeArrow, sizeArrow);
//
//         gl.Vertex(sizeArrowAx, -sizeArrow, sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, -sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, sizeArrow);
//
//         gl.Vertex(sizeArrowAx, -sizeArrow, sizeArrow);
//         gl.Vertex(sizeArrowAx, sizeArrow, -sizeArrow);
//         gl.Vertex(sizeArrowAx, -sizeArrow, -sizeArrow);
//
//         gl.Color(0f, 1f, 0);
//         gl.Vertex(0, 10, 0);
//         gl.Vertex(-sizeArrow, sizeArrowAx, -sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, -sizeArrow);
//
//         gl.Vertex(0, 10, 0);
//         gl.Vertex(sizeArrow, sizeArrowAx, -sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, sizeArrow);
//
//         gl.Vertex(0, 10, 0);
//         gl.Vertex(-sizeArrow, sizeArrowAx, sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, sizeArrow);
//
//         gl.Vertex(0, 10, 0);
//         gl.Vertex(-sizeArrow, sizeArrowAx, -sizeArrow);
//         gl.Vertex(-sizeArrow, sizeArrowAx, sizeArrow);
//
//         gl.Vertex(-sizeArrow, sizeArrowAx, sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, -sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, sizeArrow);
//
//         gl.Vertex(-sizeArrow, sizeArrowAx, sizeArrow);
//         gl.Vertex(sizeArrow, sizeArrowAx, -sizeArrow);
//         gl.Vertex(-sizeArrow, sizeArrowAx, -sizeArrow);
//
//         gl.Color(0f, 0f, 1f);
//         gl.Vertex(0, 0, 10);
//         gl.Vertex(-sizeArrow, -sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, -sizeArrow, sizeArrowAx);
//
//         gl.Vertex(0, 0, 10);
//         gl.Vertex(sizeArrow, -sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, sizeArrow, sizeArrowAx);
//
//         gl.Vertex(0, 0, 10);
//         gl.Vertex(-sizeArrow, sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, sizeArrow, sizeArrowAx);
//
//         gl.Vertex(0, 0, 10);
//         gl.Vertex(-sizeArrow, -sizeArrow, sizeArrowAx);
//         gl.Vertex(-sizeArrow, sizeArrow, sizeArrowAx);
//
//         gl.Vertex(-sizeArrow, sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, -sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, sizeArrow, sizeArrowAx);
//         
//         gl.Vertex(-sizeArrow, sizeArrow, sizeArrowAx);
//         gl.Vertex(sizeArrow, -sizeArrow, sizeArrowAx);
//         gl.Vertex(-sizeArrow, -sizeArrow, sizeArrowAx);
//
//         gl.End();
//
//         // TODO -> Captions
//
//         gl.PopMatrix();
//         gl.MatrixMode(MatrixMode.Projection);
//         gl.PopMatrix();
//         gl.MatrixMode(MatrixMode.Modelview);
//     }
//
//     private void RenderBorders()
//     {
//         var gl = BaseGraphic.GL;
//         
//         gl.MatrixMode(MatrixMode.Projection);
//         gl.PushMatrix();
//         gl.LoadIdentity();
//         gl.Viewport(0, 0, (int)BaseGraphic.ScreenSize.Width, (int)BaseGraphic.ScreenSize.Height);
//         gl.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
//         gl.MatrixMode(MatrixMode.Modelview);
//         gl.PushMatrix();
//         gl.LoadIdentity();
//         
//         gl.Color(0f, 0f, 0f);
//         gl.Begin(BeginMode.LineLoop);
//         gl.Vertex(-1.0, -1.0);
//         gl.Vertex(1.0, -1.0);
//         gl.Vertex(1.0, 1.0);
//         gl.Vertex(-1.0, 1.0);
//         gl.End();
//
//         gl.PopMatrix();
//         gl.MatrixMode(MatrixMode.Projection);
//         gl.PopMatrix();
//         gl.MatrixMode(MatrixMode.Modelview);
//     }
// }