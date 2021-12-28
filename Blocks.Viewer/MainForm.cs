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

        private static void Shutdown()
        {
            _rhinoCore.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        private static Toolbar _toolbar;
        private static forms.Splitter _splitter;

        public static BlockAssemblyInstance BlockAssemblyReference { get; private set; }
        public static BlockAssemblyInstance BlockAssemblyInstance { get; private set; }
        public static event EventHandler BlockAssemblyReferenceChanged;
        public static event EventHandler BlockAssemblyInstanceChanged;
        public static BlocksViewport ViewportL { get; private set; }
        public static BlocksViewport ViewportR { get; private set; }

        public MainForm()
        {
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
            var blockAssemblyReferenceInstance = new BlockAssemblyInstance(assembly);

            SetBlockAssemblyReference(blockAssemblyReferenceInstance);

            RefreshViewports();
        }

        public static void RefreshViewports()
        {
            ViewportL.ViewportControl.Refresh();
            ViewportR.ViewportControl.Refresh();
        }

        public static void ZoomExtents(bool refresh)
        {
            ViewportL.DisplayConduit.ZoomExtents();
            ViewportR.DisplayConduit.ZoomExtents();
            if (refresh) { RefreshViewports(); }
        }

        public static void SetBlockAssemblyInstance(BlockAssemblyInstance assembly)
        {
            BlockAssemblyInstance = assembly;
            BlockAssemblyInstanceChanged(null, EventArgs.Empty);
        }

        public static void SetBlockAssemblyReference(BlockAssemblyInstance assembly)
        {
            BlockAssemblyReference = assembly;
            BlockAssemblyReferenceChanged(null, EventArgs.Empty);
        }
    }
}
