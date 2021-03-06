// #define DEFINE_ShouldMeasureRedrawTime

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using GuiLabs.Canvas.Events;
using GuiLabs.Canvas.Renderer;
using GuiLabs.Canvas.Shapes;
using GuiLabs.Canvas.DrawOperations;
using GuiLabs.Canvas.DrawStyle;

namespace GuiLabs.Canvas
{
	public class DrawWindow : System.Windows.Forms.UserControl, IDrawWindow
	{
		#region ctor

		/// <summary>
		/// Constructor. Does practically nothing and doesn't need anything.
		/// </summary>
		public DrawWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Events

		#region Mouse

		protected void RaiseClick(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnClick(e);
			}
		}

		protected void RaiseDoubleClick(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnDoubleClick(e);
			}
		}

		protected void RaiseMouseDown(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnMouseDown(e);
			}
		}

		protected void RaiseMouseHover(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnMouseHover(e);
			}
		}

		protected void RaiseMouseMove(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnMouseMove(e);
			}
		}

		protected void RaiseMouseUp(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnMouseUp(e);
			}
		}

		protected void RaiseMouseWheel(MouseWithKeysEventArgs e)
		{
			if (DefaultMouseHandler != null)
			{
				DefaultMouseHandler.OnMouseWheel(e);
			}
		}

		#endregion

		#region Keyboard

		private void RaiseKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			if (DefaultKeyHandler != null)
			{
				DefaultKeyHandler.OnKeyDown(e);
			}
		}

		private void RaiseKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			if (DefaultKeyHandler != null)
			{
				DefaultKeyHandler.OnKeyPress(e);
			}
		}

		private void RaiseKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			if (DefaultKeyHandler != null)
			{
				DefaultKeyHandler.OnKeyUp(e);
			}
		}

		#endregion

		#endregion

        #region Component (not relevant)

        private System.ComponentModel.Container components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// To tell the control to receive all possible keys it can.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns>true to receive all possible key events.</returns>
        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            return true;
        }

        #endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DrawWindow
			// 
			this.Name = "DrawWindow";
			this.Size = new System.Drawing.Size(378, 274);
		}

		#endregion

		public event RepaintHandler Repaint;
		// public event RegionRepaintHandler RegionRepaint;

		#region Renderer

		/// <summary>
		/// DrawWindow has a Renderer.
		/// All DrawWindow instances share the same Renderer object (which is a singleton).
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IRenderer Renderer
		{
			get
			{
				return RendererSingleton.Instance;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDrawOperations DrawOperations
		{
			get
			{
				return Renderer.DrawOperations;
			}
		}

		#endregion

		#region Painting and redrawing

		#region Redraw

#if DEFINE_ShouldMeasureRedrawTime
		private IFontStyleInfo font;
#endif
		/// <summary>
		/// Whole redraw cycle - raises main Repaint event 
		/// for the clients to redraw themselves.
		/// </summary>
		/// <remarks>Uses a back buffer, which is being copied to the screen.</remarks>
		/// <param name="ToRedraw">A rectangular area which should be updated (redrawn)</param>
		public void Redraw(Rect toRedraw)
		{
			// size of the backbuffer to use
			DrawOperations.Viewport.Size.Set(this.ClientSize);

			// first, clear the buffer with the background color
			Renderer.Clear();

			// Commented out caret functionality from here.
			// Let the TextBox draw the caret instead.
			//								Kirill

			// hide the caret and see if anyone needs it
			//Caret textCursor = RendererSingleton.MyCaret;
			//textCursor.Visible = false;

			// raise main Repaint event
			// clients (those who draw on this DrawWindow)
			// handle the event and draw everything there
			// (they use the Renderer that they become over a parameter)

#if DEFINE_ShouldMeasureRedrawTime
			GuiLabs.Canvas.Utils.Timer t = new GuiLabs.Canvas.Utils.Timer();
			t.Start();
#endif

			if (Repaint != null)
				Repaint(Renderer);

#if DEFINE_ShouldMeasureRedrawTime
			t.Stop();

			if (font == null)
			{
				font = DrawOperations.Factory.ProduceNewFontStyleInfo("Verdana", 14, FontStyle.Regular);
				font.ForeColor = System.Drawing.Color.Red;
			}

			DrawOperations.DrawString(
				t.TimeElapsed.ToString(),
				DrawOperations.Viewport,
				font);
#endif

			// if someone needed the cursor, he/she turned it on
			// so we draw the cursor only if someone needs it
			// (e.g. an active TextBox control)
			//Renderer.DrawOperations.DrawCaret(textCursor);

			// finally, copy the buffer to the screen
			Renderer.RenderBuffer(this, toRedraw);
		}

		private Rect conversionRect = new Rect();
		public void Redraw(Rectangle ToRedraw)
		{
			conversionRect.Set(ToRedraw);
			Redraw(conversionRect);
		}

		public void Redraw()
		{
			conversionRect.Set(this.ClientRectangle);
			Redraw(conversionRect);
		}

		//public void Redraw(IDrawableRect ShapeToRedraw)
		//{
		//    if (RegionRepaint != null)
		//    {
		//        RegionRepaint(Renderer, ShapeToRedraw);
		//    }

		//    // finally, copy the buffer to the screen
		//    Renderer.RenderBuffer(this, ShapeToRedraw.Bounds);
		//}

		#endregion

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			Renderer.BackColor = this.BackColor;
			Redraw();
		}

		private bool mShouldRedrawOnWindowPaint = true;
		public bool ShouldRedrawOnWindowPaint
		{
			get
			{
				return mShouldRedrawOnWindowPaint;
			}
			set
			{
				mShouldRedrawOnWindowPaint = value;
			}
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			if (ShouldRedrawOnWindowPaint)
			{
				Redraw(e.ClipRectangle);
			}
			else
			{
				base.OnPaint(e);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			
		}

		#endregion

		#region Size

		///// <summary>
		///// Control size.
		///// </summary>
		//private Rect mBounds = new Rect();
		//[Browsable(false)]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//public new Rect Bounds
		//{
		//    get
		//    {
		//        return mBounds;
		//    }
		//}

		private Point mSize = new Point();
		public virtual Point GetClientSize()
		{
			return mSize;
		}

		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);

			mSize.Set(this.ClientSize.Width, this.ClientSize.Height);

			//#region Viewport
			//DrawOperations.Viewport.Size.Set(this.ClientSize);
			//#endregion
		}

		#endregion

		#region DefaultMouseHandler

		#region MouseHandler

		protected IMouseHandler mDefaultMouseHandler;
		/// <summary>
		/// If not null, all mouse events are being redirected to this object
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IMouseHandler DefaultMouseHandler
		{
			get
			{
				return mDefaultMouseHandler;
			}
			set
			{
				if (NextHandlerValid(value))
				{
					mDefaultMouseHandler = value;
				}
				else
				{
					mDefaultMouseHandler = null;
				}
			}
		}

		/// <summary>
		/// Can we set such a DefaultMouseHandler?
		/// Prevents endless recursive loops.
		/// </summary>
		/// <param name="nextHandler">Canditate to test</param>
		/// <returns>true, if setting DefaultMouseHandler to nextHandler causes no recursion.</returns>
		public bool NextHandlerValid(IMouseHandler nextHandler)
		{
			// setting to null is perfectly fine
			// (turning off the redirection)
			if (nextHandler == null)
			{
				return true;
			}

			// setting to itself would cause
			// an infinite recursion
			if (nextHandler == this)
			{
				return false;
			}

			IMouseHandler current = nextHandler;
			while (current != null)
			{
				current = current.DefaultMouseHandler;
				if (current == this)
				{
					return false;
				}
			}

			return true;
		}

		#endregion

		#region MouseEventArgs

		private MouseWithKeysEventArgs WrapMouseEventArgs(MouseEventArgs e)
		{
			return new MouseWithKeysEventArgs(e);
		}

		private System.Drawing.Point GetCursorPosition()
		{
			System.Drawing.Point CursorPos = System.Windows.Forms.Control.MousePosition;
			return this.PointToClient(CursorPos);
		}

		private MouseWithKeysEventArgs PrepareMouseArgs()
		{
			System.Drawing.Point CursorPos = GetCursorPosition();
			MouseWithKeysEventArgs Args = new MouseWithKeysEventArgs(
				Control.MouseButtons, 0, CursorPos.X, CursorPos.Y, 0);
			return Args;
		}

		#endregion

		#region Raise mouse events

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			RaiseClick(PrepareMouseArgs());
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			RaiseDoubleClick(PrepareMouseArgs());
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			RaiseMouseDown(WrapMouseEventArgs(e));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			RaiseMouseMove(WrapMouseEventArgs(e));
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			RaiseMouseUp(WrapMouseEventArgs(e));
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			RaiseMouseWheel(WrapMouseEventArgs(e));
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			RaiseMouseHover(PrepareMouseArgs());
		}

		#endregion

		#endregion

		#region DefaultKeyHandler

		#region KeyHandler

		private IKeyHandler mDefaultKeyHandler;
		/// <summary>
		/// If not null, all keyboard events 
		/// are being redirected to this object
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IKeyHandler DefaultKeyHandler
		{
			get
			{
				return mDefaultKeyHandler;
			}
			set
			{
				if (NextHandlerValid(value))
				{
					mDefaultKeyHandler = value;
				}
				else
				{
					mDefaultKeyHandler = null;
				}
			}
		}

		/// <summary>
		/// Can we set such a DefaultKeyHandler?
		/// Prevents endless recursive loops.
		/// </summary>
		/// <param name="nextHandler">Canditate to test</param>
		/// <returns>true, if setting DefaultKeyHandler to nextHandler causes no recursion.</returns>
		private bool NextHandlerValid(IKeyHandler nextHandler)
		{
			// setting to null is perfectly fine
			// (turning off the redirection)
			if (nextHandler == null)
			{
				return true;
			}

			// setting to itself would cause
			// an infinite recursion
			if (nextHandler == this)
			{
				return false;
			}

			IKeyHandler current = nextHandler;
			while (current != null)
			{
				current = current.DefaultKeyHandler;
				if (current == this)
				{
					return false;
				}
			}
			return true;
		}

		#endregion

		#region Raise keyboard events

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			RaiseKeyDown(e);
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			RaiseKeyPress(e);
		}

		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyUp(e);
			RaiseKeyUp(e);
		}

		#endregion

		#endregion
	}
}
