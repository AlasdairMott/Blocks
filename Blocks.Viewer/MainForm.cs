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

        static Rhino.UI.Controls.ViewportControl _viewportControlL;
        static Rhino.UI.Controls.ViewportControl _viewportControlR;
        static Toolbar _toolbar;

        public static BlockAssemblyInstance BlockAssemblyReference;
        public static BlockAssemblyInstance BlockAssemblyInstance;
        
        public static DisplayConduit DisplayConduitL;
        public static DisplayConduit DisplayConduitR;

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
                    demoMenu,
                }
            };

            _toolbar = new Toolbar();
            _toolbar.Show();

            _viewportControlL = new Rhino.UI.Controls.ViewportControl();
            _viewportControlL.Viewport.Name = "Left";
            SetDisplayMode(_viewportControlL.Viewport);

            _viewportControlR = new Rhino.UI.Controls.ViewportControl();
            _viewportControlR.Viewport.Name = "Right";
            SetDisplayMode(_viewportControlR.Viewport);

            DisplayConduitL = new DisplayConduit(_viewportControlL.Viewport);
            DisplayConduitL.Enabled = true;

            DisplayConduitR = new DisplayConduit(_viewportControlR.Viewport);
            DisplayConduitR.Enabled = true;

            var splitter = new forms.Splitter();
            splitter.SplitterWidth = 2;
            splitter.Position = 200;
            splitter.Panel1 = _viewportControlL;
            splitter.Panel2 = _viewportControlR;

            Content = splitter;
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

        private void SetDisplayMode(RhinoViewport viewport)
        {
            viewport.ConstructionGridVisible = false;
            viewport.ConstructionAxesVisible = false;
            viewport.WorldAxesVisible = false;

            DisplayModeDescription displayMode = DisplayModeDescription.GetDisplayModes().FirstOrDefault(d => d.EnglishName == "Blocks.Viewer");
            if (displayMode == null)
            {
                var displayId = DisplayModeDescription.CopyDisplayMode(DisplayModeDescription.ShadedId, "Blocks.Viewer");
                displayMode = DisplayModeDescription.GetDisplayMode(displayId);

                displayMode.DisplayAttributes.FillMode = DisplayPipelineAttributes.FrameBufferFillMode.SolidColor;
                displayMode.DisplayAttributes.SetFill(System.Drawing.Color.White);

                DisplayModeDescription.UpdateDisplayMode(displayMode);
            }

            viewport.DisplayMode = displayMode;
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
            BlockAssemblyReference = new BlockAssemblyInstance(assembly);

            _viewportControlL.Viewport.ZoomExtents();
            DisplayConduitL.Instance = BlockAssemblyReference;

            RefreshViewport();
        }

        public static void RefreshViewport()
        {
            _viewportControlL.Refresh();
            _viewportControlR.Refresh();
        }
    }
}
