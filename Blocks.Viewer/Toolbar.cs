﻿using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Blocks.Viewer.Display;
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
            BuildToolbar();
        }

        private void BuildToolbar()
        {
            BackgroundColor = draw.Colors.White;

            var layout = new forms.StackLayout()
            {
                Padding = new draw.Padding(2),
                Orientation = forms.Orientation.Horizontal,
                Height = 24,
            };

            var playButton = new forms.Button { 
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Play), 
                Style = "toolbar-button",
            };
            playButton.Click += PlayButton_Click;

            var randomizeButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Randomize),
                Style = "toolbar-button",
            };
            randomizeButton.Click += RandomizeButton_Click;

            var zoomExtentsButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.ZoomExtents),
                Style = "toolbar-button",
            };
            zoomExtentsButton.Click += ZoomExtentsButton_Click;

            var ShowText = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Labels),
                Style = "toolbar-button",
            };
            ShowText.Click += ShowText_Click;

            SeedStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 10, Width = 48 };
            StepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 50, Width = 48 };

            var graphParameters = new forms.Button
            {
                Size = new draw.Size(20, 20),
                BackgroundColor = draw.Colors.White
            };

            layout.Items.Add(playButton);
            layout.Items.Add(randomizeButton);
            layout.Items.Add(zoomExtentsButton);
            layout.Items.Add(ShowText);

            layout.Items.Add("  Seed:");
            layout.Items.Add(SeedStepper);
            layout.Items.Add("  Steps:");
            layout.Items.Add(StepsStepper);
            layout.Items.Add(new forms.StackLayoutItem { Expand = true });
            //layout.Items.Add(graphParameters);

            Content = layout;
        }

        private void RandomizeButton_Click(object sender, EventArgs e)
        {
            SeedStepper.Value = _random.Next(0, 100);
            StepsStepper.Value = _random.Next(0, 100);
            
            Run();
            MainForm.RefreshViewports();
        }

        private void ZoomExtentsButton_Click(object sender, EventArgs e) => MainForm.ZoomExtents(true);

        private void ShowText_Click(object sender, EventArgs e)
        {
            MainForm.ViewportL.BlockDisplayConduit.ToggleTextDisplay();
            MainForm.ViewportR.BlockDisplayConduit.ToggleTextDisplay();
            MainForm.RefreshViewports();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Run();
            MainForm.RefreshViewports();
        }

        private void Run()
        {
            if (MainForm.Reference == null) return;

            var generator = new GenerateFromTransitions((int)SeedStepper.Value);
            var transitions = new Transitions(MainForm.Reference.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, (int)StepsStepper.Value);

            MainForm.SetGenerated(outputAssembly);
        }
    }
}
