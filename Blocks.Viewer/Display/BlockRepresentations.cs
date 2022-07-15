using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Blocks.Common.Parameters;
using Blocks.Viewer.Data;
using System;

namespace Blocks.Viewer.Display
{
    public class BlockRepresentations
    {
        private Graph2dInstance _graph;
        private BlockAssemblyInstance _blockAssemblyInstance;
        private Skeleton _skeleton;
        private DisplayMode _mode;

        public BlockAssembly BlockAssembly { get; private set; }
        public BlockAssemblyInstance BlockAssemblyInstance { 
            get 
            { 
                if (_blockAssemblyInstance == null)
                {
                    _blockAssemblyInstance = new BlockAssemblyInstance(BlockAssembly);
                }
                return _blockAssemblyInstance;
            } 
        }
        public Graph2dInstance Graph
        {
            get
            {
                if (_graph == null)
                {
                    ComputeGraph(Preferences.GraphGeneratorParameters);
                }
                return _graph;
            }
        }
        public Skeleton Skeleton 
        { 
            get 
            {
                if (_skeleton == null)
                {
                    _skeleton = new Skeleton(BlockAssembly);
                }
                return _skeleton;
            } 
        }
        public IDrawable Get(DisplayMode mode)
        {
            _mode = mode;

            switch (_mode)
            {
                case DisplayMode.Solid: return BlockAssemblyInstance;
                case DisplayMode.Wire: return Skeleton;
                case DisplayMode.Graph: return Graph;
                default: throw new NotImplementedException();
            }
        } 
        public BlockRepresentations(BlockAssembly blockAssembly)
        {
            BlockAssembly = blockAssembly;
        }

        public void ComputeGraph(GraphGeneratorParameters parameters)
        {
            var graphGenerator = new GraphGenerator();
            var graph2d = graphGenerator.Generate(BlockAssembly, parameters);
            _graph = new Graph2dInstance(graph2d);
        }
    }
}
