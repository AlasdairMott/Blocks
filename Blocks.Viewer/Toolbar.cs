using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using draw = Eto.Drawing;
using forms = Eto.Forms;

namespace Blocks.Viewer
{
    public class Toolbar : forms.Panel
    {
        private Random _random = new Random();
        public forms.NumericStepper SeedStepper { get; private set; }
        public forms.NumericStepper StepsStepper { get; private set; }
        public Toolbar()
        {
            BackgroundColor = draw.Colors.White;
            BuildToolbar();
        }

        private void BuildToolbar()
        {
            var layout = new forms.StackLayout()
            {
                Padding = new draw.Padding(2),
                Orientation = forms.Orientation.Horizontal,
                Height = 24,
            };

            var playButton = new forms.Button { 
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Play), 
                Size = new draw.Size(20,20),
                BackgroundColor = draw.Colors.White };
            playButton.Click += PlayButton_Click;

            var randomizeButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Randomize),
                Size = new draw.Size(20, 20),
                BackgroundColor = draw.Colors.White
            };
            randomizeButton.Click += RandomizeButton_Click;

            var zoomExtentsButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.ZoomExtents),
                Size = new draw.Size(20, 20),
                BackgroundColor = draw.Colors.White
            };
            zoomExtentsButton.Click += ZoomExtentsButton_Click;

            SeedStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 10, Width = 48 };
            StepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 50, Width = 48 };

            layout.Items.Add(playButton);
            layout.Items.Add(randomizeButton);
            layout.Items.Add(zoomExtentsButton);
            layout.Items.Add(new forms.StackLayoutItem{Expand = true});
            layout.Items.Add("Seed:");
            layout.Items.Add(SeedStepper);
            layout.Items.Add("  Steps:");
            layout.Items.Add(StepsStepper);

            Content = layout;
        }

        private void RandomizeButton_Click(object sender, EventArgs e)
        {
            SeedStepper.Value = _random.Next(0, 100);
            StepsStepper.Value = _random.Next(0, 100);
            Run();
            MainForm.ViewportR.DisplayConduit.ZoomExtents();
            MainForm.RefreshViewports();
        }

        private void ZoomExtentsButton_Click(object sender, EventArgs e) => MainForm.ZoomExtents(true);

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Run();
            MainForm.RefreshViewports();
        }

        private void Run()
        {
            var generator = new GenerateFromTransitions((int)SeedStepper.Value);
            var transitions = new Transitions(MainForm.BlockAssemblyReference.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, (int)StepsStepper.Value);
            var outputAssemblyInstance = new BlockAssemblyInstance(outputAssembly);

            MainForm.SetBlockAssemblyInstance(outputAssemblyInstance);
        }
    }
}
