using Blocks.Solvers;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Components
{
    public class ReadTransitionsComponent : GH_Component
	{
		public ReadTransitionsComponent()
		  : base("Read Transitions", "Nickname","Description", "Blocks", "Subcategory")
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
			pManager.AddGenericParameter("Transitions", "T", "Transitions that can be used to create a block assembly", GH_ParamAccess.item);
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

			var reader = new ReadTransitions();
			var transitions = reader.Read(instances.ToList(), distanceThreshold);

			DA.SetData(0, transitions);
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