using Blocks.Common.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Linq;

namespace Blocks.Common.Functions
{
    public static class GeometryHelpers
    {
        /// <summary>
        /// Get the geometry from an BlockInstance as a mesh.
        /// </summary>
        /// <param name="instance">BlockInstance to extract the mesh from.</param>
        /// <returns>All the BlockInstance's geomery as a single joined mesh.</returns>
        public static Mesh GetBlockInstanceMesh(BlockInstance instance)
        {
            var geometry = instance.GetGeometry();
            var mesh = new Mesh();
            foreach (var geo in geometry)
            {
                var geometryMesh = GetGeometryMesh(geo);
                if (geometryMesh != null) mesh.Append(geometryMesh);
            }
            return mesh;
        }

        /// <summary>
        /// Extract the mesh from any piece of geometry.
        /// </summary>
        /// <param name="geometry">Geometry to extract the mesh from.</param>
        /// <returns>The GeometryBase as a mesh.</returns>
        /// <remarks>From https://discourse.mcneel.com/t/get-mesh-for-any-geometry-object-in-3dm-file/133006/3. </remarks>
        public static Mesh GetGeometryMesh(GeometryBase geometry)
        {
            Mesh rc = null;

            if (geometry is Mesh mesh)
            {
                rc = mesh;
            }
            else if (geometry is Extrusion extrusion)
            {
                var extrusionMesh = extrusion.GetMesh(MeshType.Any);
                if (null != extrusionMesh)
                    rc = extrusionMesh;
            }
            else if (geometry is Brep brep)
            {
                var brepMesh = new Mesh();
                foreach (var brepFace in brep.Faces)
                {
                    var brepFaceMesh = brepFace.GetMesh(MeshType.Any);
                    if (null != brepFaceMesh)
                        brepMesh.Append(brepFaceMesh);
                }
                if (brepMesh.IsValid)
                    rc = brepMesh;
            }

            if (null != rc)
            {
                rc.Normals.ComputeNormals();
                rc.Compact();
                if (!rc.IsValid)
                    rc = null;
            }

            return rc;
        }
    }
}
