using System;
using System.Windows.Forms;
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
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new GameOverlay.Windows.OverlayWindow(0, 0, width, height)
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

        private bool _isRunning = true;
        public void Run()
        {
            var abc = _graphics.CreateSolidBrush(99, 32, 123, 150);

            int i = 100;
            while (_isRunning)
            {
                _graphics.BeginScene();
                _graphics.ClearScene();

                _graphics.FillRectangle(abc, i, i, i + 100, i + 100);

                _graphics.EndScene();

                i++;
            }

            _graphics.BeginScene();
            _graphics.ClearScene();
            _graphics.EndScene();
        }

        public void Stop()
        {
            _isRunning = false;
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
    }
}
