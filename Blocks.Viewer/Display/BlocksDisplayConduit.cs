using Rhino.Display;

namespace Blocks.Viewer.Display
{
    public class BlocksDisplayConduit : DisplayConduit
    {
        private BlocksViewport _parent;
        public bool TextIsVisible { get; set; }
        public IDrawable Instance 
        { 
            get 
            {
                if (_parent.BlockVisibiltyDropdown.SelectedIndex == 0)
                {
                    return MainForm.Reference?.Get((DisplayMode) _parent.DisplayStyleDropdown.SelectedIndex);
                } else
                {
                    return MainForm.Generated?.Get((DisplayMode)_parent.DisplayStyleDropdown.SelectedIndex);
                }
            } 
        }

        public BlocksDisplayConduit(BlocksViewport parent)
        {
            _parent = parent;
        }

        public void ToggleTextDisplay() => TextIsVisible = !TextIsVisible;

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.CalculateBoundingBox(e);
            if (Instance != null)
            {
                e.IncludeBoundingBox(Instance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.PostDrawObjects(e);
            Instance?.PostDraw(e);

            if (TextIsVisible && Instance is IDrawLabel label)
            {
                label.DrawText(e);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.PreDrawObjects(e);
            Instance?.PreDraw(e);
        }

        public void ZoomExtents() { 
            if (Instance != null)
            {
                var bbox = Instance.BoundingBox;
                var amount = 5;

                bbox.Inflate(amount);

                _parent.ViewportControl.Viewport.Camera35mmLensLength = 50;
                _parent.ViewportControl.Viewport.ZoomBoundingBox(bbox);
            }
        }
    }
}
