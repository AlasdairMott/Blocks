using System;
using Xunit;
using Rhino.Geometry;

namespace Blocks.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			//Arrange
			var point1 = new Point3d(0, 0, 0);
			var point2 = new Point3d(0, 0, 600);

			//Act
			var distance = point1.DistanceTo(point2);

			//Assert
			Assert.InRange(distance, 0, 1000);
		}
	}
}
