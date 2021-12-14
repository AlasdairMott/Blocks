using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Components
{
    public class CreateAssemblyComponent : GH_Component
    {
        public CreateAssemblyComponent()
          : base("Create Assembly", "Nickname", "Description", "Blocks", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Block Definitions", "B-D", "Block definitions to create the assembly from", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iterations", "I", "Iterations for markov", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Seed", "S", "Seed for markov", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
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
            var blockDefinitions = new List<BlockDefinition>();
            DA.GetDataList(0, blockDefinitions);

            var iterations = 10;
            DA.GetData(1, ref iterations);

            var seed = 0;
            DA.GetData(2, ref seed);

            var placements = PlaceGeometry(blockDefinitions, seed, iterations);

            DA.SetDataList(0, placements.Select(p => p.Geometry));
            DA.SetDataList(1, placements.Select(p => p.Transform));
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


        private IEnumerable<(Transform Transform, GeometryBase Geometry)> PlaceGeometry(IEnumerable<BlockDefinition> blocks, int seed, int iterations)
        {
            var placements = new List<KeyValuePair<BlockDefinition, Transform>>();
            var random = new Random(seed);

            var item = blocks.ElementAt(random.Next(0, blocks.Count()));
            placements.Add(new KeyValuePair<BlockDefinition, Transform>(item, Transform.Identity));

            for (var i = 0; i < iterations; i++)
            {
                var index = random.Next(0, placements.Count());
                var existing = placements.ElementAt(index);

                var next = blocks.First(b => b.Index == existing.Key.Index);
                if (next.Relationships.Count() == 0) { continue; }

                var nextRelationship = next.Next(random);
                var nextTransform = existing.Value * nextRelationship.Transform;

                placements.Add(new KeyValuePair<BlockDefinition, Transform>(nextRelationship.Definition, nextTransform));
            }

            var geometries = new List<GeometryBase>();
            foreach (var placement in placements)
            {
                var geometry = placement.Key.Geometry;
                foreach (var g in geometry)
                {
                    var dup = g.Duplicate();
                    dup.Transform(placement.Value);
                    geometries.Add(dup);
                }
            }

            return placements.Select(p => p.Value).Zip(geometries, (Transform, Geometry) => (Transform, Geometry));
        }
    }
}
