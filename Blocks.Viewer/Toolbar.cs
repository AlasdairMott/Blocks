using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Display;
using System;
using System.IO;
using System.Linq;
using draw = Eto.Drawing;
using forms = Eto.Forms;

namespace Blocks.Viewer
{
    public class Toolbar : forms.Form
    {
        public forms.NumericStepper SeedStepper { get; private set; }
        public forms.NumericStepper StepsStepper { get; private set; }
        public Toolbar()
        {
            Title = "Blocks.Viewer.Toolbar";
            ClientSize = new draw.Size(240, 40);
            Topmost = true;
            WindowStyle = forms.WindowStyle.None;
            BackgroundColor = draw.Colors.White;
            Opacity = 0.9;

            MouseDown += Toolbar_MouseDown;
            MouseMove += Toolbar_MouseMove;

            BuildToolbar();
        }

        private draw.PointF _start;
        private void Toolbar_MouseMove(object sender, forms.MouseEventArgs e)
        {
            if (e.Buttons.HasFlag(forms.MouseButtons.Primary))
            {
                var delta = e.Location - _start;
                var point = new draw.Point((int) (Location.X + delta.X), (int) (Location.Y + delta.Y));
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

            var icon = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Play);
            var playButton = new forms.Button { Image = icon, Width = 24 };
            playButton.Click += PlayButton_Click;

            SeedStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 10, Width = 48 };
            StepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 50, Width = 48 };

            layout.AddRow(playButton, "Seed:", SeedStepper, "Steps:", StepsStepper, null);

            Content = layout;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            var generator = new GenerateFromTransitions((int)SeedStepper.Value);
            var transitions = new Transitions(MainForm.BlockAssemblyReference.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, (int)StepsStepper.Value);
            var outputAssemblyInstance = new BlockAssemblyInstance(outputAssembly);

            MainForm.BlockAssemblyInstance = outputAssemblyInstance;
            MainForm.DisplayConduitR.Instance = outputAssemblyInstance;

            MainForm.RefreshViewport();
        }
    }
}
