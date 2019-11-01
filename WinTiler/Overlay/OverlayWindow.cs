using System;
using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace WinTiler.Overlay
{
    public class OverlayWindow
    {
        private readonly GameOverlay.Windows.OverlayWindow _window;
        private readonly Graphics _graphics;

        private SolidBrush _gray;

        public OverlayWindow()
        {
            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new GameOverlay.Windows.OverlayWindow(0, 0, 800, 600)
            {
                IsTopmost = true,
                IsVisible = true,
            };

            // handle this event to resize your Graphics surface
            _window.SizeChanged += _window_SizeChanged;

            // initialize a new Graphics object
            // set everything before you call _graphics.Setup()
            _graphics = new Graphics
            {
                MeasureFPS = true,
                Height = _window.Height,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = true,
                Width = _window.Width,
                WindowHandle = IntPtr.Zero
            };
        }

        ~OverlayWindow()
        {
            // don't forget to free resources
            _graphics.Dispose();
            _window.Dispose();
        }

        public void Initialize()
        {
            // creates the window using the settings we applied to it in the constructor
            _window.CreateWindow();

            _graphics.WindowHandle = _window.Handle; // set the target handle before calling Setup()
            _graphics.Setup();

            _gray = _graphics.CreateSolidBrush(255, 144, 0, 100);
        }

        public void Run()
        {
            var gfx = _graphics; // little shortcut

            while (true)
            {
                gfx.BeginScene(); // call before you start any drawing
                gfx.ClearScene(_gray); // set the background of the scene (can be transparent)

                gfx.EndScene();
            }
        }

        private void _window_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            if (_graphics == null) return;

            if (_graphics.IsInitialized)
            {
                // after the Graphics surface is initialized you can only use the Resize method in order to enqueue a size change
                _graphics.Resize(e.Width, e.Height);
            }
            else
            {
                // otherwise just set its members
                _graphics.Width = e.Width;
                _graphics.Height = e.Height;
            }
        }

        public void Move()
        {
            _window.X += 1;
            _window.Y += 1;
        }
    }
}
