namespace Blocks.Common.Parameters
{
    public class BlockAssemblyEdgeReaderParameters
    {
        public double CollisionArea { get; set; }
        public double DistanceThreshold { get; set; }

        public BlockAssemblyEdgeReaderParameters(double collisionArea, double distanceThreshold)
        {
            CollisionArea = collisionArea;
            DistanceThreshold = distanceThreshold;
        }
    }
}
