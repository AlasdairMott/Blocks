using Blocks.Common.Generators;
using System;
using System.Linq;
using draw = Eto.Drawing;
using forms = Eto.Forms;

namespace Blocks.Viewer
{
    public class Toolbar : forms.Panel
    {
        private Random _random = new Random();
        public forms.NumericStepper SeedStepper { get; private set; }
        public forms.NumericStepper StepsStepper { get; private set; }
        public forms.DropDown GeneratorList { get; private set; }
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

            var assembly = typeof(IBlockAssemblyGenerator).Assembly;
            var generators = assembly.GetTypes().Where(p => typeof(IBlockAssemblyGenerator).IsAssignableFrom(p) && p!= typeof(IBlockAssemblyGenerator));

            GeneratorList = new forms.DropDown();
            foreach (var generator in generators)
            {
                GeneratorList.Items.Add(generator.Name);
            }
            GeneratorList.SelectedIndex = 0;

            var playButton = new forms.Button { 
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Play), 
                Style = "toolbar-button",
            };
            playButton.Click += (s,e) => Run();

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
            StepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 3, Width = 48 };

            var graphParameters = new forms.Button
            {
                Size = new draw.Size(20, 20),
                BackgroundColor = draw.Colors.White
            };

            var nextButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Next),
                Style = "toolbar-button",
            };
            nextButton.Click += (s, e) => { StepsStepper.Value++; Run(); };

            var previousButton = new forms.Button
            {
                Image = Rhino.UI.EtoExtensions.ToEto(Viewer.Properties.Resources.Previous),
                Style = "toolbar-button",
            };
            previousButton.Click += (s, e) => { StepsStepper.Value--; Run(); };

            layout.Items.Add(playButton);
            layout.Items.Add(randomizeButton);
            layout.Items.Add(zoomExtentsButton);
            layout.Items.Add(ShowText);

            layout.Items.Add("  Seed:");
            layout.Items.Add(SeedStepper);
            layout.Items.Add("  Steps:");
            layout.Items.Add(StepsStepper);
            
            layout.Items.Add(previousButton);
            layout.Items.Add(nextButton);

            layout.Items.Add(new forms.StackLayoutItem { Expand = true });
            layout.Items.Add(GeneratorList);
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

        private void Run()
        {
            Commands.Generate.Run(GeneratorList.SelectedKey, (int)SeedStepper.Value, (int)StepsStepper.Value, Data.Preferences.UseGroundPlane);
            MainForm.RefreshViewports();
        }
    }
}
