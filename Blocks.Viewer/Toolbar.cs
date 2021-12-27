using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using draw = Eto.Drawing;
using forms = Eto.Forms;

namespace Blocks.Viewer
{
    public class Toolbar : forms.Form
    {
        private Random _random = new Random();
        public forms.NumericStepper SeedStepper { get; private set; }
        public forms.NumericStepper StepsStepper { get; private set; }
        public Toolbar()
        {
            Title = "Blocks.Viewer.Toolbar";
            ClientSize = new draw.Size(290, 40);
            Topmost = true;
            WindowStyle = forms.WindowStyle.None;
            BackgroundColor = draw.Colors.White;

            MouseDown += Toolbar_MouseDown;
            MouseMove += Toolbar_MouseMove;

            BuildToolbar();
        }

        private draw.PointF _start;
        private void Toolbar_MouseMove(object sender, forms.MouseEventArgs e)
        {
            var delta = e.Location - _start;
            var point = new draw.Point((int)(Location.X + delta.X), (int)(Location.Y + delta.Y));

            if (e.Buttons.HasFlag(forms.MouseButtons.Primary) && 
                !SeedStepper.Bounds.Contains(new draw.Point((int)e.Location.X, (int)e.Location.Y)) &&
                !StepsStepper.Bounds.Contains(new draw.Point((int)e.Location.X, (int)e.Location.Y)))
            {
                Location = point;
            }
        }

        private void Toolbar_MouseDown(object sender, forms.MouseEventArgs e) => _start = e.Location;

        private void BuildToolbar()
        {
            var layout = new forms.DynamicLayout
            {
                Padding = new draw.Padding(8),
                Spacing = new draw.Size(5, 5),
            };

            var playButton = new forms.Button { 
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Play), 
                Width = 24, 
                BackgroundColor = draw.Colors.White };
            playButton.Click += PlayButton_Click;

            var randomizeButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Randomize),
                Width = 24,
                BackgroundColor = draw.Colors.White
            };
            randomizeButton.Click += RandomizeButton_Click;

            var zoomExtentsButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.ZoomExtents),
                Width = 24,
                BackgroundColor = draw.Colors.White
            };
            zoomExtentsButton.Click += ZoomExtentsButton_Click;

            SeedStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 10, Width = 48 };
            StepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 50, Width = 48 };

            layout.AddRow(playButton, randomizeButton, zoomExtentsButton, null, "Seed:", SeedStepper, "Steps:", StepsStepper, null);

            Content = layout;
        }

        private void RandomizeButton_Click(object sender, EventArgs e)
        {
            SeedStepper.Value = _random.Next(0, 100);
            StepsStepper.Value = _random.Next(0, 100);
            Run();
            MainForm.DisplayConduitR.ZoomExtents();
            MainForm.RefreshViewport();
        }

        private void ZoomExtentsButton_Click(object sender, EventArgs e) => MainForm.ZoomExtents(true);

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Run();
            MainForm.RefreshViewport();
        }

        private void Run()
        {
            var generator = new GenerateFromTransitions((int)SeedStepper.Value);
            var transitions = new Transitions(MainForm.BlockAssemblyReference.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, (int)StepsStepper.Value);
            var outputAssemblyInstance = new BlockAssemblyInstance(outputAssembly);

            MainForm.BlockAssemblyInstance = outputAssemblyInstance;
            MainForm.DisplayConduitR.SetInstance(outputAssemblyInstance);
        }
    }
}
