using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
	public class Relationship
	{
		public BlockDefinition Definition { get; set; }
		public Transform Transform { get; set; }
		public double Strength { get; set; }

		public Relationship(BlockDefinition instanceDefinition, Transform transform)
		{
			Definition = instanceDefinition;
			Transform = transform;
		}
	}
}
