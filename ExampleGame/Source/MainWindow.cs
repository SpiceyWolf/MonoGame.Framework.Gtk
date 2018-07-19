using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Gtk;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public partial class MainWindow : Gtk.Window
    {
        private int anim1, anim2, anim3, anim4;
        private SpriteBatch sb;
        private Texture2D tex1;
        private Texture2D tex2;

        public MainWindow() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();

			// Render Widget 1
			InitSurface(ref rsFace1, Color.CornflowerBlue, 10);
			rsFace1.Render += (sender, e) => { DrawAnim(ref tex1, ref anim1); };

			// Render Widget 2
			InitSurface(ref rsFace2, Color.PaleVioletRed, 100);
			rsFace2.Render += (sender, e) => { DrawAnim(ref tex2, ref anim2); };

			// Render Widget 3
			InitSurface(ref rsFace3, Color.Khaki, 1000);
			rsFace3.Render += (sender, e) => { DrawAnim(ref tex2, ref anim3); };

            // Render Widget 4
			InitSurface(ref rsFace4, Color.SeaGreen, 10000);
			rsFace4.Render += (sender, e) => { DrawAnim(ref tex1, ref anim4); };

            // Initialize Graphical Objects
            sb = new SpriteBatch(UniversalBackend.GraphicsDevice);
            tex1 = UniversalBackend.Content.Load<Texture2D>("Content/1");
            tex2 = UniversalBackend.Content.Load<Texture2D>("Content/2");
        }

		private void InitSurface(ref RenderWidget surface, Color color, int interval)
		{
			surface.BackColor = color;
            surface.AutoDraw = true;
            surface.AutoDrawInterval = interval;
		}

		private void DrawAnim(ref Texture2D tex, ref int anim)
		{
			// Make sure animation stays within the walk frames
	        if (anim > 3) anim = 0;

            sb.Begin();
                
	        // Draw the texture with the animation index
	        sb.Draw(
	                texture: tex,
	                position: Vector2.Zero,
	                color: Color.White,
	                sourceRectangle: new Rectangle(anim * 48, 146, 48, 48));

            sb.End();

	        // Iterate Animation
	        anim++;
		}
    }
}