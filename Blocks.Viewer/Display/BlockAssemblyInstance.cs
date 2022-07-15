using Blocks.Common.Functions;
using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Blocks.Viewer.Display
{
    public class BlockAssemblyInstance : IDrawable
    {
        private Mesh _mesh = new Mesh();
        protected Line[] _meshWires = new Line[0];
        private DisplayMaterial _material = new DisplayMaterial { Diffuse = Color.White };
        public BoundingBox BoundingBox { get; private set; } = BoundingBox.Empty;
        public BlockAssemblyInstance(BlockAssembly assembly)
        {
            foreach (var instance in assembly.BlockInstances)
            {
                var mesh = GeometryHelpers.GetBlockInstanceMesh(instance);
                _mesh.Append(mesh);
            }
            _meshWires = GetMeshWires(_mesh);

            BoundingBox = _mesh.GetBoundingBox(true);
        }

        private BlockAssemblyInstance() { }
        public static BlockAssemblyInstance Empty => new BlockAssemblyInstance();

        private Line[] GetMeshWires(Mesh mesh)
        {
            var lines = new List<Line>();
            foreach (var island in mesh.ExplodeAtUnweldedEdges())
            {
                lines.AddRange(island.GetNakedEdges().SelectMany(i => i.GetSegments()));
            }
            return lines.ToArray();
        }

        public void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawMeshWires(_mesh, Color.Black, 3);
        }

        public void PostDraw(DrawEventArgs e)
        {
            e.Display.DrawMeshShaded(_mesh, _material);
            e.Display.DrawLines(_meshWires, Color.Black);
        }
    }
}
