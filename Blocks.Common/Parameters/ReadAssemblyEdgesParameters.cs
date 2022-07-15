using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks.Common.Parameters
{
    public class ReadAssemblyEdgesParameters
    {
        public double CollisionArea { get; set; }
        public double DistanceThreshold { get; set; }

        public ReadAssemblyEdgesParameters(double collisionArea, double distanceThreshold)
        {
            CollisionArea = collisionArea;
            DistanceThreshold = distanceThreshold;
        }
    }
}
