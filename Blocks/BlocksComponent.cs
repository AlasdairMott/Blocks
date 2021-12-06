using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
	public class BlocksComponent : GH_Component
	{
		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public BlocksComponent()
		  : base("Blocks", "Nickname",
			  "Description",
			  "Blocks", "Subcategory")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("Blocks", "B", "Blocks", GH_ParamAccess.list);
			pManager.AddNumberParameter("Distance threshold", "D", "Distance threshold", GH_ParamAccess.item);
			pManager.AddIntegerParameter("Iterations", "I", "Iterations for markov", GH_ParamAccess.item);
			pManager.AddIntegerParameter("Seed", "S", "Seed for markov", GH_ParamAccess.item);

			pManager[2].Optional = true;
			pManager[3].Optional = true;
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddTextParameter("Block name", "N", "Block name", GH_ParamAccess.list);
			pManager.AddTransformParameter("Block xform", "P", "Block xform", GH_ParamAccess.list);
			pManager.AddGeometryParameter("Geometry", "G", "Geometry", GH_ParamAccess.list);
			pManager.AddTransformParameter("Placement xforms", "T", "Placement xforms", GH_ParamAccess.list);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var instanceIds = new List<Guid>();
			if (!DA.GetDataList(0, instanceIds)) { return; }
			if (instanceIds.Any(id => id == null)) { AddRuntimeMessage( GH_RuntimeMessageLevel.Warning,"Null in list"); return; }
			

			var doc = Rhino.RhinoDoc.ActiveDoc;
			var instances = instanceIds.Select(id => doc.Objects.FindId(id)).Cast<InstanceObject>();
			if (instances.Count() == 0) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No instances found"); return; }

			var distanceThreshold = 1.0;
			if (!DA.GetData(1, ref distanceThreshold)) { return; }

			var iterations = 10;
			DA.GetData(2, ref iterations);

			var seed = 0;
			DA.GetData(3, ref seed);

			//need to make a tranform comparer
			var comparer = new RelationshipComparer();
			var blocks = new Dictionary<InstanceDefinition, Block>();

			//for each block, read it's closest neighbours, build a list of connections and transforms?
			foreach (var instance in instances)
			{
				Block block;
				if (blocks.ContainsKey(instance.InstanceDefinition))
				{
					block = blocks[instance.InstanceDefinition];
				} else
				{
					block = new Block(instance.InstanceDefinition);
					blocks.Add(instance.InstanceDefinition, block);
				}

				var relations = block.Relationships;

				foreach (var other in instances)
				{
					if (other.InsertionPoint.DistanceTo(instance.InsertionPoint) > distanceThreshold) { continue; }
					if (other.Id == instance.Id) { continue; }

					var xform1 = instance.InstanceXform;
					var xform2 = other.InstanceXform;

					var plane1 = Plane.WorldXY;
					plane1.Transform(xform1);

					var plane2 = Plane.WorldXY;
					plane2.Transform(xform2);

					xform1.TryGetInverse(out var xformInverse);

					var transform = xformInverse * xform2;

					var key = new Relationship(other.InstanceDefinition, transform);
					if (relations.Contains(key))
					{
						relations.FirstOrDefault(r => r.Equals(key)).Strength += 1;
					} else
					{
						key.Strength = 1;
						block.AddRelationship(key);
					}
				}
			}

			foreach (var block in blocks)
			{
				block.Value.NormalizeRelationships();
			}

			var placements = new List<KeyValuePair<InstanceDefinition, Transform>>();
			var random = new Random(seed);

			var item = blocks.ElementAt(random.Next(0, blocks.Count()));
			placements.Add(new KeyValuePair<InstanceDefinition, Transform>(item.Value.Definition, Transform.Identity));

			for (var i = 0; i < iterations; i++)
			{
				var index = random.Next(0, placements.Count());
				var existing = placements.ElementAt(index);

				var next = blocks.First(b => b.Key.Id == existing.Key.Id).Value;
				if (next.Relationships.Count() == 0) { continue; }

				var nextRelationship = next.Next(random);
				var nextTransform = existing.Value * nextRelationship.Transform;

				placements.Add(new KeyValuePair<InstanceDefinition, Transform>(nextRelationship.Definition, nextTransform));
			}

			var geometries = new List<GeometryBase>();
			foreach (var placement in placements)
			{
				var geometry = placement.Key.GetObjects();
				foreach (var g in geometry)
				{
					g.Geometry.Transform(placement.Value);
					geometries.Add(g.Geometry);
				}
			}

			DA.SetDataList(0, instances.Select(b => b.InstanceDefinition.Name));
			DA.SetDataList(1, instances.Select(b => b.InstanceXform));
			DA.SetDataList(2, geometries);
			DA.SetDataList(3, placements.Select(p => p.Value));
		}

		/// <summary>
		/// Provides an Icon for every component that will be visible in the User Interface.
		/// Icons need to be 24x24 pixels.
		/// </summary>
		protected override System.Drawing.Bitmap Icon => Properties.Resources.Relationship;

		/// <summary>
		/// Each component must have a unique Guid to identify it. 
		/// It is vital this Guid doesn't change otherwise old ghx files 
		/// that use the old ID will partially fail during loading.
		/// </summary>
		public override Guid ComponentGuid => new Guid("8bf660e6-1e47-4a69-a750-1c66529d8fc9");
	}
}
