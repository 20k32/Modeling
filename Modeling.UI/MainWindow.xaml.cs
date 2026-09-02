using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Modeling.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            // Wire up canvas events
            canvas.Draw += Canvas_Draw;
            canvas.CreateResources += Canvas_CreateResources;

            // Set refresh rate: smaller time = higher FPS
            // Default is 16.67ms (~60 FPS)
            canvas.TargetElapsedTime = TimeSpan.FromMilliseconds(8.33);  // ~120 FPS

            // Or for even higher:
            // canvas.TargetElapsedTime = TimeSpan.FromMilliseconds(4.17);  // ~240 FPS
        }

        private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            // Initialize any resources here (brushes, images, etc.)
            // This is called once before the first Draw event
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            // Get the drawing session
            var ds = args.DrawingSession;

            // Access timing information (automatically updated each frame)
            var totalTime = args.Timing.TotalTime;
            var deltaTime = args.Timing.ElapsedTime;
            var frameCount = args.Timing.FrameCount;

            // Example: Draw a red rectangle
            ds.FillRectangle(100, 100, 200, 200, Colors.Red);

            // Example: Draw a blue circle
            ds.FillCircle(500, 300, 100, Colors.Blue);

            // Example: Draw text showing frame info
            ds.DrawText($"Frame: {frameCount} | FPS: {(1.0 / deltaTime.TotalSeconds):F1}", 200, 50, Colors.Black);

            // Example: Draw a line
            ds.DrawLine(0, 0, 800, 600, Colors.Green, 2);
        }
    }
}
