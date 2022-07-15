namespace Blocks.Common.Parameters
{
    public class GraphGeneratorParameters
    {
        public double SpringConstant { get; set; }
        public double RestingLength { get; set; }
        public double RepulsionFactor { get; set; }
        public double RepsulionRadius { get; set; }
        public double CoolingFactor { get; set; }
        public int MaxIterations { get; set; }

        public GraphGeneratorParameters(double springConstant, double restingLength, double repulsionFactor, double repulsionRadius, double coolingFactor, int maxIterations)
        {
            SpringConstant = springConstant;
            RestingLength = restingLength;
            RepulsionFactor = repulsionFactor;
            RepsulionRadius = repulsionRadius;
            CoolingFactor = coolingFactor;
            MaxIterations = maxIterations;
        }
    }
}
