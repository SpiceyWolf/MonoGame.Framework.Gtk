using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Forms;
using Gdk;
using widgetType = Gtk.Image;

namespace Microsoft.Xna.Framework.Gtk
{
    [System.ComponentModel.ToolboxItem(true)]
    public class RenderWidget : widgetType
    {
        private Timer _autoDraw = new Timer();
        private Color _clearColor;
        private Viewport _view;
		private bool _needUpdate = true;

        /// <summary>
        /// If true, will automatically redraw the
        /// surface on the specified interval.
        /// </summary>
        public bool AutoDraw
        {
            get { return _autoDraw.Enabled; }
            set { _autoDraw.Enabled = value; }
        }

        /// <summary>
        /// Specifies the interval in milliseconds
        /// to redraw the surface by.
        /// </summary>
        public int AutoDrawInterval
        {
            get { return _autoDraw.Interval; }
            set
            {
                if (value == 0) return;
                _autoDraw.Interval = value;
            }
        }

        /// <summary>
        /// Changes the background color of the surface renderer when in runtime.
        /// </summary>
        public Color BackColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        public RenderWidget()
        {
            _autoDraw.Enabled = true;
            // Initialize default properties
            DoubleBuffered = false;
            _view = new Viewport(0, 0,
                Allocation.Width,
                Allocation.Height);

            // Setup Events
			_autoDraw.Tick += (sender, e) => { _needUpdate = true; QueueDraw(); };
            Render += OnRender;
            SizeAllocated += (o, args) =>
            {
                _view = new Viewport(0, 0,
                    args.Allocation.Width,
                    args.Allocation.Height);
            };
        }

        protected override bool OnExposeEvent(EventExpose ev)
        {
			if (!_needUpdate) return base.OnExposeEvent(ev);

            // Begin the drawing
            UniversalBackend.BeginDraw(Allocation.Width, Allocation.Height);

            // Clear device
            UniversalBackend.GraphicsDevice.Clear(_clearColor);

            // Setup default view
            UniversalBackend.GraphicsDevice.Viewport = _view;

            // Invoke event based render calls
            Render?.Invoke(null, EventArgs.Empty);

            // End the drawing
            UniversalBackend.EndDraw();

            // Present the graphics to our widget
            Pixbuf = UniversalBackend.Present();

			// Mark that we have updated
			_needUpdate = false;

            // Do Gdk Drawing.
            return base.OnExposeEvent(ev);
        }

        public override void Dispose()
        {
            _autoDraw.Dispose();
            base.Dispose();
        }

        public event EventHandler Render;
        public virtual void OnRender(object sender, EventArgs e) { }
    }
}