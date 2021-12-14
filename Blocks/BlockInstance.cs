using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks
{
    public class BlockInstance
    {
        public Transform Transform { get; set;}
        public BlockDefinition BlockDefinition { get; set;}
    }
}
