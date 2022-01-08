using Rhino.Display;
using Rhino.Geometry;

namespace Blocks.Viewer.Display
{
    public interface IDrawable
    {
        BoundingBox BoundingBox { get; }
        void PreDraw(DrawEventArgs e);
        void PostDraw(DrawEventArgs e);
    }
}
