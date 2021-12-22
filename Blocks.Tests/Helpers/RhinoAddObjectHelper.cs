using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace Blocks.Tests.Helpers
{
    public static class RhinoAddObjectHelper
    {
        public static Guid[] AddPlane(Rhino3dmHelper helper, Plane plane, string name)
        {
            var objectIds = new Guid[3];
            objectIds[0] = helper.Objects.AddLine(
                new Line(plane.Origin, plane.XAxis), 
                new ObjectAttributes { 
                    ObjectColor = Color.Red, 
                    ColorSource = ObjectColorSource.ColorFromObject,
                    Name = name
                });

            objectIds[1] = helper.Objects.AddLine(
                new Line(plane.Origin, plane.YAxis),
                new ObjectAttributes
                {
                    ObjectColor = Color.Green,
                    ColorSource = ObjectColorSource.ColorFromObject,
                    Name = name
                });

            objectIds[2] = helper.Objects.AddLine(
                new Line(plane.Origin, plane.ZAxis),
                new ObjectAttributes
                {
                    ObjectColor = Color.Blue,
                    ColorSource = ObjectColorSource.ColorFromObject,
                    Name = name
                });

            return objectIds;
        }
    }
}
