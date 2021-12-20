using System;
using Xunit;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Collections;
using Blocks.Objects;
using Xunit.Abstractions;

namespace Blocks.Tests
{
    [Collection("RhinoTestingCollection")]
    public class RelationshipTests
    {
        private readonly ITestOutputHelper _output;

        public RelationshipTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CalculateRelativeTransform_IsSuccessful()
        {
            //Arrange
            var plane1 = new Plane(new Point3d(0.1, 45, 5), new Vector3d(2.2, -4.4, 1));
            var plane2 = new Plane(new Point3d(-3.2, 5.1, 5.1), new Vector3d(-0.5, 1.2, -1.1));

            var transform1 = Transform.PlaneToPlane(Plane.WorldXY, plane1);
            var transform2 = Transform.PlaneToPlane(Plane.WorldXY, plane2);

            var point1 = plane1.Origin;
            var point2 = plane2.Origin;

            //Act
            var result = Relationship.CalculateRelativeTransform(transform1, transform2);
            point1.Transform(result.Transform);
            point2.Transform(result.Inverse);

            //Assert
            var tolerance = 0.01;
            Assert.InRange(point1.DistanceTo(plane2.Origin), 0, tolerance);
            Assert.InRange(point2.DistanceTo(plane1.Origin), 0, tolerance);
        }

        [Fact(Skip = "Not Implemented")]
        public void InvertRelationship_ItInverts()
        {

        }
    }
}
