using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Gdk;

namespace Microsoft.Xna.Framework.Gtk
{
    public static class UniversalBackend
    {
        /// <summary>
        /// Gets or Sets the backend ContentManager.
        /// </summary>
        public static ContentManager Content
        {
            get { return MasterWindow.Instance.Content; }
            set { MasterWindow.Instance.Content = value; }
        }

        /// <summary>
        /// Gets the backend GraphicsDevice.
        /// </summary>
        public static GraphicsDevice GraphicsDevice
        {
            get { return MasterWindow.Instance.GraphicsDevice; }
        }

        /// <summary>
        /// Gets the backend GraphicsDeviceManager.
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return MasterWindow.Instance.DeviceManager; }
        }

        /// <summary>
        /// Begins the drawing process making present unusable.
        /// </summary>
        public static bool BeginDraw(int width, int height)
        {
            return MasterWindow.Instance.BeginRender(width, height);
        }

        /// <summary>
        /// Finalizes the drawing and makes present usable.
        /// </summary>
        public static void EndDraw()
        {
            MasterWindow.Instance.EndRender();
        }

        /// <summary>
        /// Returns a bitmap of the BackBuffer to be drawn to a control.
        /// </summary>
        public static Pixbuf Present()
        {
            return MasterWindow.Instance.BackBuffer;
        }

        private class MasterWindow : Game
        {
            internal GraphicsDeviceManager DeviceManager;
            internal Pixbuf BackBuffer;
            private bool isDrawing;

            #region Singleton
            private static readonly Lazy<MasterWindow> _instance = new Lazy<MasterWindow>
                (() => new MasterWindow(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

            internal static MasterWindow Instance { get { return _instance.Value; } }

            // Constructor
            private MasterWindow() : base()
            {
                // Init Graphics Device
                DeviceManager = new GraphicsDeviceManager(this);

                // Make sure the entire game window is created
                RunOneFrame();
            }

            protected override void Dispose(bool disposing)
            {
                if (BackBuffer != null)
                {
                    BackBuffer.Dispose();
                    BackBuffer = null;
                }

                DeviceManager.Dispose();
                base.Dispose(disposing);
            }
            #endregion

            internal bool BeginRender(int width, int height)
            {
                if (isDrawing)
                    throw new Exception("Draw operation already in progress!");

                // Clear BackBuffer if doing new render
                if (BackBuffer != null)
                {
                    BackBuffer.Dispose();
                    BackBuffer = null;
                }

                // Prepair the screen
                DeviceManager.PreferredBackBufferWidth = width;
                DeviceManager.PreferredBackBufferHeight = height;
                DeviceManager.ApplyChanges();

                isDrawing = true;
                return true;
            }

            internal void EndRender()
            {
                // Exit if not drawing anything
                if (!isDrawing) return;

                // Create Surface Image
                var width = DeviceManager.PreferredBackBufferWidth;
                var height = DeviceManager.PreferredBackBufferHeight;

                var bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                var bmpData = bmp.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppRgb);
                var pixelData = new int[width * height];

                // Get buffer data
                GraphicsDevice.GetBackBufferData(pixelData);
                for (int i = 0; i < pixelData.Length; i++)
#pragma warning disable // Caused by bitwise function requiring uint to int conversion
                    pixelData[i] = (int)( // Swap bgra - rgba
                        (pixelData[i] & 0x000000ff) << 16 |
                        (pixelData[i] & 0x0000FF00) |
                        (pixelData[i] & 0x00FF0000) >> 16 |
                        (pixelData[i] & 0xFF000000));
#pragma warning disable

                // Convert to bitmap
                Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length);
                bmp.UnlockBits(bmpData);

                // Convert to pixbuf
                using (var stream = new System.IO.MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    BackBuffer = new Pixbuf(stream);
                    bmp.Dispose();
                }

                // Mark as done drawing.
                isDrawing = false;
            }
        }
    }
}
