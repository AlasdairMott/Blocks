namespace Blocks.Common.Objects
{
    public class TransitionInstance : Transition
    {
        public BlockInstance FromInstance { get; private set; }
        public BlockInstance ToInstance { get; private set; }

        public TransitionInstance(BlockInstance from, BlockInstance to) : base(from, to) {
            FromInstance = from;
            ToInstance = to;
        }
    }
}
