namespace SharpPlot.Core.Drawing.Interactivity.Interfaces;

public interface IDragger
{
    bool CanDrag { get; }
    
    void StartDrag(double x, double y, double z);
    void Drag(double x, double y, double z);
    void EndDrag();
}