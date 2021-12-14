using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Components
{
    public class ReadBlocksComponent : GH_Component
	{
		public ReadBlocksComponent()
		  : base("Read Blocks", "Nickname","Description", "Blocks", "Subcategory")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("Blocks", "B", "Blocks", GH_ParamAccess.list);
			pManager.AddNumberParameter("Distance threshold", "D", "Distance threshold", GH_ParamAccess.item);
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddGenericParameter("Block Definitions", "B-D", "Block definitions", GH_ParamAccess.list);
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
			if (instances.Any(i => i == null)) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No instances found"); return; }

			var distanceThreshold = 1.0;
			if (!DA.GetData(1, ref distanceThreshold)) { return; }

			var blocks = LearnRelationships(instances, distanceThreshold);

			DA.SetDataList(0, blocks);
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

		private IEnumerable<BlockDefinition> LearnRelationships(IEnumerable<InstanceObject> instances, double distanceThreshold)
        {
			//need to make a tranform comparer
			var comparer = new RelationshipComparer();
			var blocks = new Dictionary<InstanceDefinition, BlockDefinition>();

			//for each block, read it's closest neighbours, build a list of connections and transforms
			foreach (var instance in instances)
			{
				BlockDefinition block;
				if (blocks.ContainsKey(instance.InstanceDefinition))
				{
					block = blocks[instance.InstanceDefinition];
				}
				else
				{
					block = new BlockDefinition(instance.InstanceDefinition);
					blocks.Add(instance.InstanceDefinition, block);
				}

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

					var key = new Relationship(other.InstanceDefinition.ToDefinition(), transform);
					if (block.Relationships.Contains(key))
					{
						var existing = block.FindRelationship(key);
						existing.Strength += 1;
					}
					else
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

			return blocks.Select(b => b.Value);
		}
	}
}