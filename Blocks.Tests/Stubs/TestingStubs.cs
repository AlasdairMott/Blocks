using Blocks.Common.Objects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Tests.Stubs
{
    public static class TestingStubs
    {
		private static readonly Dictionary<string, List<GeometryBase>> _geometries = new Dictionary<string, List<GeometryBase>>();
		private static readonly Dictionary<string, BlockDefinition> _definitions = new Dictionary<string, BlockDefinition>();
		private static readonly Dictionary<string, Transform> _transforms = new Dictionary<string, Transform>();
		private static readonly Dictionary<string, BlockInstance> _instances = new Dictionary<string, BlockInstance>();
		private static readonly Dictionary<string, Transition> _transitions = new Dictionary<string, Transition>();

		public static IReadOnlyDictionary<string, List<GeometryBase>> Geometries { 
			get 
			{ 
				if (!_geometries.Any())
				{
                    _geometries.Add("geometry-A", new List<GeometryBase>());
					_geometries.Add("geometry-B", new List<GeometryBase>());
					_geometries.Add("geometry-C", new List<GeometryBase>());
				}
				return _geometries;
			} 
		}

		public static IReadOnlyDictionary<string, BlockDefinition> Definitions
        {
			get
			{
				if (!_definitions.Any())
				{
					_definitions.Add("A", new BlockDefinition(Geometries["geometry-A"], "0"));
					_definitions.Add("B", new BlockDefinition(Geometries["geometry-B"], "1"));
					_definitions.Add("C", new BlockDefinition(Geometries["geometry-C"], "2"));
				}
				return _definitions;
			}

		}

		public static IReadOnlyDictionary<string, Transform> Transforms
		{
			get
			{
				if (!_transforms.Any())
				{
					_transforms.Add("Identity", Transform.Identity);
					_transforms.Add("ChangePlane1", Transform.PlaneToPlane(Plane.WorldXY, Plane.WorldZX));
					_transforms.Add("ChangePlane2", Transform.PlaneToPlane(new Plane(new Point3d(10, 10, 10), new Vector3d(12, 4, -5)), Plane.WorldZX));
				}
				return _transforms;
			}
		}

		public static IReadOnlyDictionary<string, BlockInstance> Instances
        {
			get
            {
				if (!_instances.Any())
                {
					_instances.Add("A1", new BlockInstance(Definitions["A"], Transforms["Identity"]));
					_instances.Add("A2", new BlockInstance(Definitions["A"], Transforms["ChangePlane1"]));
					_instances.Add("B1", new BlockInstance(Definitions["B"], Transforms["ChangePlane2"]));
				}
				return _instances;
            }
        }

		public static IReadOnlyDictionary<string, Transition> Transitions
        {
			get
            {
				if (!_transitions.Any())
                {
					_transitions.Add("A1-A1", new Transition(Instances["A1"], Instances["A1"]));
					_transitions.Add("A1-A2", new Transition(Instances["A1"], Instances["A2"]));
					_transitions.Add("A1-B1", new Transition(Instances["A1"], Instances["B1"]));
					_transitions.Add("A2-A1", new Transition(Instances["A2"], Instances["A1"]));
					_transitions.Add("A2-A2", new Transition(Instances["A2"], Instances["A2"]));
					_transitions.Add("A2-B1", new Transition(Instances["A2"], Instances["B1"]));
					_transitions.Add("B1-A2", new Transition(Instances["B1"], Instances["A2"]));
				}
				return _transitions;
            }
        }
	}
}
