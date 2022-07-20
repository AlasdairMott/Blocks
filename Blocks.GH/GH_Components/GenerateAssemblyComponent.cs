using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.GH.Components
{
    public class GenerateAssemblyComponent : GH_Component
    {
        public GenerateAssemblyComponent()
          : base("Create Assembly", "A", "Create a block assembly", "Blocks", "Main")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "Assembly to base the new assembly on", GH_ParamAccess.item);
            pManager.AddMeshParameter("Obstacles", "O", "Obstacles", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Steps", "Stp", "Steps for markov", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Seed", "Sd", "Seed for markov", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Block Instance", "B", "Block instance", GH_ParamAccess.list);
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
            var inputAssembly = new BlockAssembly();
            if (!DA.GetData(0, ref inputAssembly)) { return; };

            var obstacleMeshes = new List<Mesh>();
            DA.GetDataList(1, obstacleMeshes);

            var obstacles = new Mesh();
            obstacles.Append(obstacleMeshes.Where(o => o!= null));

            var steps = 10;
            DA.GetData(2, ref steps);

            var seed = 0;
            DA.GetData(3, ref seed);

            var transitions = new Transitions(inputAssembly);
            var generator = new FromTransitionsGenerator(transitions, obstacles, seed, steps);
            var outputAssembly = generator.Generate();

            DA.SetDataList(0, outputAssembly.BlockInstances);
            DA.SetDataList(1, outputAssembly.GetGeometry());
            DA.SetDataList(2, outputAssembly.BlockInstances.Select(b => b.Transform));
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
        public override Guid ComponentGuid => new Guid("6943A647-4A70-42F5-ABC9-8D3A7FCB4723");
    }
}
