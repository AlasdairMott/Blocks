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
    class MainForm : forms.Form
    {
        static Rhino.Runtime.InProcess.RhinoCore _rhinoCore;

        public static void Run()
        {
            _rhinoCore = new Rhino.Runtime.InProcess.RhinoCore(new string[] { "-appmode" }, Rhino.Runtime.InProcess.WindowStyle.Hidden);
            MainForm mf = new MainForm();
            mf.Show();

            for (int i = 0; i < System.Windows.Application.Current.Windows.Count; i++)
            {
                var window = System.Windows.Application.Current.Windows[i];
                if (window.IsVisible)
                {
                    window.Closed += (s, e) => Shutdown();
                    break;
                }
            }
            System.Windows.Forms.Application.Run();
        }

        static void Shutdown()
        {
            _rhinoCore.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        static Rhino.UI.Controls.ViewportControl _viewportControl;
        static forms.NumericStepper _seedStepper;
        static forms.NumericStepper _stepsStepper;

        public static BlockAssemblyInstance BlockAssemblyInstance;
        public static DisplayConduit DisplayConduit;
        public MainForm()
        {
            Title = "Blocks.Viewer";
            ClientSize = new draw.Size(400, 400);

            var demoMenu = new forms.ButtonMenuItem { Text = "&Demos" };
            BuildDemosMenu(demoMenu.Items);
            Menu = new forms.MenuBar()
            {
                Items =
                {
                    new forms.ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        {
                            new forms.ButtonMenuItem(new forms.Command((s,e)=>OpenFileDialog())) { Text = "Open..." },
                        }
                    },
                    demoMenu
                }
            };

            var layout = new forms.DynamicLayout();

            var icon = Rhino.UI.EtoExtensions.ToEto(Blocks.Viewer.Properties.Resources.Play);
            var playButton = new forms.Button { Image = icon, Width = 28 };
            playButton.Click += PlayButton_Click;

            _seedStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 10};
            _stepsStepper = new forms.NumericStepper() { DecimalPlaces = 0, MinValue = 0, Value = 50};

            layout.AddSeparateRow(playButton, "Seed:", _seedStepper, "Steps:", _stepsStepper, null);

            _viewportControl = new Rhino.UI.Controls.ViewportControl();
            SetDisplayMode();
            DisplayConduit = new DisplayConduit();
            DisplayConduit.Enabled = true;
            layout.AddSeparateRow(_viewportControl);

            Content = layout;
        }

        private void BuildDemosMenu(forms.MenuItemCollection collection)
        {
            var demosFolder = GetDebugExamplesDirectory();
            var files = Directory.GetFiles(demosFolder, "*.3dm*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var menuitem = new forms.ButtonMenuItem((s, e) =>
                {
                    OpenFile(file);
                });
                menuitem.Text = Path.GetFileName(file);
                collection.Add(menuitem);
            }
        }

        private string GetDebugExamplesDirectory()
        {
            var codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            var directory = new DirectoryInfo(Path.GetDirectoryName(codeBase).Substring(6));
            var repository = directory.Parent.Parent.Parent;
            return Path.Combine(repository.FullName, "examples");
        }

        private void SetDisplayMode()
        {
            //Rhino.RhinoApp.RunScript("Oneview Enabled=No Enter", false);

            _viewportControl.Viewport.ConstructionGridVisible = false;
            _viewportControl.Viewport.ConstructionAxesVisible = false;
            _viewportControl.Viewport.WorldAxesVisible = false;

            DisplayModeDescription displayMode = DisplayModeDescription.GetDisplayModes().FirstOrDefault(d => d.EnglishName == "Blocks.Viewer");
            if (displayMode == null)
            {
                var displayId = DisplayModeDescription.CopyDisplayMode(DisplayModeDescription.ShadedId, "Blocks.Viewer");
                displayMode = DisplayModeDescription.GetDisplayMode(displayId);

                displayMode.DisplayAttributes.FillMode = DisplayPipelineAttributes.FrameBufferFillMode.SolidColor;
                displayMode.DisplayAttributes.SetFill(System.Drawing.Color.White);

                DisplayModeDescription.UpdateDisplayMode(displayMode);
            }
            
            _viewportControl.Viewport.DisplayMode = displayMode;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            var generator = new GenerateFromTransitions((int) _seedStepper.Value);
            var transitions = new Transitions(BlockAssemblyInstance.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, (int)_stepsStepper.Value);

            BlockAssemblyInstance = new BlockAssemblyInstance(outputAssembly);

            _viewportControl.Refresh();
        }

        void OpenFileDialog()
        {
            var ofd = new forms.OpenFileDialog();
            ofd.Filters.Add(new forms.FileFilter("Rhino 3dm", ".3dm"));
            if (ofd.ShowDialog(this) == forms.DialogResult.Ok)
            {
                OpenFile(ofd.FileName);
            }
        }

        void OpenFile(string filename)
        {
            Title = $"Blocks.Viewer ({filename})";

            var reader = new Common.Readers.ReadBlockAssembly();
            var rhinoFile = Rhino.FileIO.File3dm.Read(filename);

            var referenceObjects = rhinoFile.Objects.Where(o => o.Geometry.ObjectType == ObjectType.InstanceReference);
            var referenceGeometry = referenceObjects.Select(r => r.Geometry as InstanceReferenceGeometry);

            var blockDefinitions = rhinoFile.AllInstanceDefinitions.Select(d =>
            {
                var geometry = d.GetObjectIds().Select(id => rhinoFile.Objects.FindId(id).Geometry);
                return new BlockDefinition(geometry, d.Name);
            }).ToList();

            var instances = referenceGeometry.Select(r => {
                var referenceDefinition = rhinoFile.AllInstanceDefinitions.First(d => d.Id == r.ParentIdefId);
                var blockDefinition = blockDefinitions.First(b => b.Name == referenceDefinition.Name);
                var xform = r.Xform;
                return new BlockInstance(blockDefinition, xform);
            });

            var assembly = reader.Read(instances.ToList(), 50);
            BlockAssemblyInstance = new BlockAssemblyInstance(assembly);

            _viewportControl.Viewport.ZoomExtents();
            _viewportControl.Refresh();
        }
    }
}
