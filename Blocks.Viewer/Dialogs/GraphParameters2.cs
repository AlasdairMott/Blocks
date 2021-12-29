using Blocks.Viewer.Data;
using Eto.Drawing;
using Eto.Forms;

namespace Blocks.Viewer.Dialogs
{
    public class GraphParameters2 : Panel
    {
        public GraphParameters2() : base()
        {
            BuildPanel();
        }

        private void BuildPanel()
        {
            var parameters = Preferences.GraphGeneratorParameters;

            var stepper_springConstant = new NumericStepper() { Value = parameters.SpringConstant, MaximumDecimalPlaces = 3 };
            stepper_springConstant.ValueChanged += (sender, e) => parameters.SpringConstant = stepper_springConstant.Value;

            var stepper_restLength = new NumericStepper() { Value = parameters.RestingLength, MaximumDecimalPlaces = 3 };
            stepper_restLength.ValueChanged += (sender, e) => parameters.RestingLength = stepper_restLength.Value;

            var stepper_repulsionFactor = new NumericStepper() { Value = parameters.RepulsionFactor, MaximumDecimalPlaces = 3 };
            stepper_repulsionFactor.ValueChanged += (sender, e) => parameters.RepulsionFactor = stepper_repulsionFactor.Value;

            var stepper_threshold = new NumericStepper() { Value = parameters.Threshold, MaximumDecimalPlaces = 3 };
            stepper_threshold.ValueChanged += (sender, e) => parameters.Threshold = stepper_threshold.Value;

            var stepper_Iterations = new NumericStepper() { MinValue = 0, Value = parameters.MaxIterations };
            stepper_Iterations.ValueChanged += (sender, e) => parameters.MaxIterations = (int) stepper_Iterations.Value;

            var runButton = new Button() { Text = "Run" };
            runButton.Click += (sender, e) =>
            {
                MainForm.Reference?.ComputeGraph(Preferences.GraphGeneratorParameters);
                MainForm.Generated?.ComputeGraph(Preferences.GraphGeneratorParameters);
                MainForm.RefreshViewports();
            };

            Content = new DynamicLayout()
            {
                Padding = new Padding(10),
                Spacing = new Size(5, 5),
                Rows =
                {
                    new DynamicRow{"Spring Constant", stepper_springConstant },
                    new DynamicRow{"Resting Length", stepper_restLength },
                    new DynamicRow{"Repulsion Factor", stepper_repulsionFactor },
                    new DynamicRow{"Threshold", stepper_threshold },
                    new DynamicRow{"Iterations", stepper_Iterations },
                    new DynamicRow{runButton },
                    new DynamicRow{},
                }
            };
        }
    }
}
