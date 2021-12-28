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
        private Graph _graph;
        public BlockAssembly BlockAssembly { get; private set; }
        public Mesh Mesh { get; private set; } = new Mesh();
        public Line[] MeshWires;
        public DisplayMaterial Material { get; private set; } = new DisplayMaterial { Diffuse = Color.White };
        public BoundingBox BoundingBox { get; private set; }
        public Graph Graph
        {
            get { if (_graph == null) { _graph = new Graph(); _graph.Build(BlockAssembly); } return _graph; }
        }
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
            BoundingBox.Inflate(BoundingBox.Diagonal.Length / 2);
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

        public void PreDraw(DrawEventArgs e, BlockAssemblyDisplayMode mode)
        {
            switch (mode)
            {
                case BlockAssemblyDisplayMode.Solid: 
                    e.Display.DrawMeshWires(Mesh, Color.Black, 3); 
                    break;
            }
        }

        public void PostDraw(DrawEventArgs e, BlockAssemblyDisplayMode mode)
        {
            switch (mode)
            {
                case BlockAssemblyDisplayMode.Solid:
                    e.Display.DrawMeshShaded(Mesh, Material);
                    e.Display.DrawLines(MeshWires, Color.Black);
                    break;
                case BlockAssemblyDisplayMode.Graph:
                    e.Display.DrawLines(Graph.Edges, Color.Black, 2);
                    for (int i = 0; i < Graph.Vertices.Length; i++)
                    {
                        e.Display.Draw2dText(Graph.Labels[i], Color.Black, Graph.Vertices[i], true);
                        //e.Display.Draw2dRectangle()
                    }
                    break;
            }
        }
    }
}
