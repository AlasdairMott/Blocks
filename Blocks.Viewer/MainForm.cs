using Microsoft.Win32;
using System;

namespace Blocks.Viewer
{
    class MainForm : Eto.Forms.Form
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
        public MainForm()
        {
            Title = "Blocks.Viewer";
            ClientSize = new Eto.Drawing.Size(400, 400);
            _viewportControl = new Rhino.UI.Controls.ViewportControl();
            Content = _viewportControl;

            var viewMenu = new Eto.Forms.ButtonMenuItem { Text = "&View" };

            Menu = new Eto.Forms.MenuBar()
            {
                Items =
                {
                    new Eto.Forms.ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        {
                            new Eto.Forms.ButtonMenuItem(new Eto.Forms.Command((s,e)=>OpenFile())) { Text = "Open..." }
                        }
                    },
                    viewMenu
                }
            };

        }

        void OpenFile()
        {
            var ofd = new Eto.Forms.OpenFileDialog();
            ofd.Filters.Add(new Eto.Forms.FileFilter("Rhino 3dm", ".3dm"));
            if (ofd.ShowDialog(this) == Eto.Forms.DialogResult.Ok)
            {
                Title = $"Rhino.Inside ({ofd.FileName})";
                Rhino.RhinoDoc.Open(ofd.FileName, out bool alreadyOpen);
                _viewportControl.Viewport.ZoomExtents();
                _viewportControl.Refresh();
            }
        }
    }
}
