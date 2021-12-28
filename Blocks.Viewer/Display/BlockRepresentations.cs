using Blocks.Common.Generators;
using Blocks.Common.Objects;
using System;

namespace Blocks.Viewer.Display
{
    public class BlockRepresentations
    {
        private Graph2dInstance _graph;
        private BlockAssemblyInstance _blockAssemblyInstance;
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
                    var graphGenerator = new GraphGenerator();
                    var graph2d = graphGenerator.Generate(BlockAssembly, 0, 0, 0, 0);
                    _graph = new Graph2dInstance(graph2d);
                }
                return _graph;
            }
        }

        public IDrawable Get(DisplayMode mode)
        {
            _mode = mode;

            switch (_mode)
            {
                case DisplayMode.Solid: return BlockAssemblyInstance;
                case DisplayMode.Wire: return Graph;
                case DisplayMode.Graph: return Graph;
                default: throw new NotImplementedException();
            }
        } 
        public BlockRepresentations(BlockAssembly blockAssembly)
        {
            BlockAssembly = blockAssembly;
        }

    }
}
