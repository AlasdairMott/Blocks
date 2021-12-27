using Blocks.Common.Functions;
using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Blocks.Viewer
{
    public class BlockAssemblyInstance
    {
        public BlockAssembly BlockAssembly { get; private set; }
        public Mesh Mesh { get; private set; } = new Mesh();
        public Line[] MeshWires;
        public DisplayMaterial Material { get; private set; } = new DisplayMaterial { Diffuse = Color.White };
        public BoundingBox BoundingBox { get; private set; }
        public BlockAssemblyInstance(BlockAssembly assembly)
        {
            BlockAssembly = assembly;

            foreach (var instance in assembly.BlockInstances)
            {
                var mesh = GeometryHelpers.GetBlockInstanceMesh(instance);
                Mesh.Append(mesh);
            }
            MeshWires = GetMeshWires(Mesh);

            BoundingBox = Mesh.GetBoundingBox(true);
        }

        public static Line[] GetMeshWires(Mesh mesh)
        {
            var lines = new List<Line>();
            foreach (var island in mesh.ExplodeAtUnweldedEdges())
            {
                lines.AddRange(island.GetNakedEdges().SelectMany(i => i.GetSegments()));
            }
            return lines.ToArray();
        }
    }
}
