using System;
using System.Collections.Generic;

namespace Blocks.Common.Generators.ConstraintSolvers
{
    /// <summary>
    /// Used to track the assembly during its generation.
    /// </summary>
    /// <remarks>Each propogation step is a state. At each step there is a dictionary mapping states to their entanglements.</remarks>
    public class ConstraintDebugger
    {
        public List<Dictionary<int, (bool collapsed, bool eliminated, List<int> entanglements)>> States;

        public void Bake(IEnumerable<State> states)
        {
            var docObjects = Rhino.RhinoDoc.ActiveDoc.Objects;
            foreach (var state in states)
            {
                throw new NotImplementedException();
            }
        }

        public static System.Drawing.Color ColorFromState(State state)
        {
            int r = Math.Abs(state.Key * 3 % 255);
            int g = Math.Abs(state.Key * 7 % 255);
            int b = Math.Abs(state.Key * 13 % 255);
            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }
}
