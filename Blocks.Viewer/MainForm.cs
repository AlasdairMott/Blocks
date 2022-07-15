using Blocks.Common.Objects;
using Blocks.Viewer.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.IO;
using System.Linq;
using draw = Eto.Drawing;
using forms = Eto.Forms;

namespace Blocks.Viewer
{
    class MainForm : forms.Form
    {
        private static Toolbar _toolbar;
        private static forms.Splitter _splitter;

        public static BlockRepresentations Reference { get; private set; }
        public static BlockRepresentations Generated { get; private set; }
        public static BlocksViewport ViewportL { get; private set; }
        public static BlocksViewport ViewportR { get; private set; }

        public MainForm()
        {
            BuildForm();
            Data.Preferences.CreateDefaults();
        }

        private void BuildForm()
        {
            BuildStyles();

            Title = "Blocks.Viewer";
            ClientSize = new draw.Size(800, 400);

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
                    new forms.ButtonMenuItem
                    {
                        Text = "&Commands",
                        Items = 
                        {
                            new Commands.GraphParameters(),
                        }
                    }
                    
                }
            };

            _toolbar = new Toolbar();

            ViewportL = new BlocksViewport("Left");
            ViewportR = new BlocksViewport("Right");

            _splitter = new forms.Splitter
            {
                SplitterWidth = 2,
                Position = Bounds.Width / 2,
                Panel1 = ViewportL,
                Panel2 = ViewportR,
            };

            var layout = new forms.TableLayout() { Rows = { _toolbar, _splitter } };

            Content = layout;

            SizeChanged += MainForm_SizeChanged;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e) => _splitter.Position = Bounds.Width / 2;

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

        private void OpenFileDialog()
        {
            var ofd = new forms.OpenFileDialog();
            ofd.Filters.Add(new forms.FileFilter("Rhino 3dm", ".3dm"));
            if (ofd.ShowDialog(this) == forms.DialogResult.Ok)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(string filename)
        {
            Title = $"Blocks.Viewer ({filename})";

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

            var reader = new Common.Readers.BlockAssemblyReader();
            var parameters = Data.Preferences.BlockAssemblyReaderParameters;
            var assembly = reader.Read(instances.ToList(), parameters);

            SetReference(assembly);

            RefreshViewports();
        }

        public static void RefreshViewports()
        {
            ViewportL.ViewportControl.Refresh();
            ViewportR.ViewportControl.Refresh();
        }

        public static void ZoomExtents(bool refresh)
        {
            ViewportL.BlockDisplayConduit.ZoomExtents();
            ViewportR.BlockDisplayConduit.ZoomExtents();
            if (refresh) { RefreshViewports(); }
        }

        public static void BuildStyles()
        {
            Eto.Style.Add<forms.Button>("toolbar-button", button => {
                button.Size = new draw.Size(20, 20);
                button.BackgroundColor = draw.Colors.White;
            });

            Eto.Style.Add<forms.Button>("viewport-button", button => {
                button.Size = new draw.Size(22, 22);
                button.BackgroundColor = draw.Colors.White;
            });
        }

        public static void SetGenerated(BlockAssembly assembly)
        {
            Generated = new BlockRepresentations(assembly);
        }

        public static void SetReference(BlockAssembly assembly)
        {
            Reference = new BlockRepresentations(assembly);
        }
    }
}
