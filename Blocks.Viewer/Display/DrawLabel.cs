using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;

namespace Blocks.Viewer.Display
{
    public static class DrawLabel
    {
        private static readonly int _textHeight = 16;
        private static readonly string _fontFace = "Consolas";
        private static readonly int _padding = 4;
        private static readonly Point2d _shift = new Point2d(0, -20);

        public static void Draw(DisplayPipeline displayPipeline, Point3d location, string text)
        {
            var location2d = displayPipeline.Viewport.WorldToClient(location) + _shift;
            var rectangle = displayPipeline.Measure2dText(text, location2d, true, 0, _textHeight, _fontFace);
            var mask = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, -rectangle.Height);

            mask.Inflate(_padding, _padding);
            displayPipeline.Draw2dRectangle(mask, Color.Black, 1, Color.White);
            displayPipeline.Draw2dText(text, Color.Black, location2d, true, _textHeight);
        }
    }
}
