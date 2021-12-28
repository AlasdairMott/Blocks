using Rhino.Display;
using Rhino.Geometry;

namespace Blocks.Viewer.Display
{
    public class GraphInstance : IDrawable
    {
        public Point3d[] Vertices { get; private set; }
        public Line[] Edges { get; private set; }
        public string[] Labels { get; private set; }

        public BoundingBox BoundingBox => throw new System.NotImplementedException();

        public GraphInstance()
        {
            
        }

        public void PreDraw(DrawEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void PostDraw(DrawEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
