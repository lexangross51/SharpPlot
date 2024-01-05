namespace SharpPlot.Drawing.Interactivity.Interfaces;

public interface IMouseTracker
{
    double X { get; }
    double Y { get; }
    
    void Update(double x, double y);
}