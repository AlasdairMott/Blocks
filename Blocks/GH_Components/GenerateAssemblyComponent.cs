using Blocks.Solvers;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Components
{
    public class GenerateAssemblyComponent : GH_Component
    {
        public GenerateAssemblyComponent()
          : base("Create Assembly", "Nickname", "Description", "Blocks", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Block Pool", "P", "Block definitions to create the assembly from", GH_ParamAccess.item);
            pManager.AddMeshParameter("Obstacles", "O", "Obstacles", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Steps", "S", "Steps for markov", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Seed", "S", "Seed for markov", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Block Instance", "B-I", "Block instance", GH_ParamAccess.list);
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
            var pool = new BlockPool();
            if (!DA.GetData(0, ref pool)) { return; };

            var obstacleMeshes = new List<Mesh>();
            DA.GetDataList(1, obstacleMeshes);

            var obstacles = new Mesh();
            obstacles.Append(obstacleMeshes.Where(o => o!= null));

            var steps = 10;
            DA.GetData(2, ref steps);

            var seed = 0;
            DA.GetData(3, ref seed);

            var generator = new GenerateBlockAssembly(seed);
            var assembly = generator.PlaceGeometry(pool, obstacles, steps);

            DA.SetDataList(0, assembly.BlockInstances);
            DA.SetDataList(1, assembly.GetGeometry());
            DA.SetDataList(2, assembly.BlockInstances.Select(b => b.Transform));
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
