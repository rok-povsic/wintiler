using System;
using System.Threading;
using System.Windows.Forms;
using GameOverlay.Drawing;

namespace WinTiler.Overlay
{
    public class OverlayWindow
    {
        private readonly GameOverlay.Windows.OverlayWindow _window;
        private readonly Graphics _graphics;

        private bool _isDrawing = false;
        private Thread _overlayThread;
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public OverlayWindow()
        {
            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new GameOverlay.Windows.OverlayWindow(0, 0, FullScreen.ScreenWidth, FullScreen.ScreenHeight)
            {
                IsTopmost = true,
                IsVisible = true,
            };

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
        }

        public void Run(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;

            if (!_isDrawing)
            {
                StartDrawing();
            }
        }

        private void StartDrawing()
        {
            _isDrawing = true;

            _overlayThread = new Thread(() =>
            {
                var brush = _graphics.CreateSolidBrush(99, 32, 123, 150);

                while (_isDrawing)
                {
                    _graphics.BeginScene();
                    _graphics.ClearScene();

                    _graphics.FillRectangle(brush, _left, _top, _right, _bottom);

                    _graphics.EndScene();
                }

                _graphics.BeginScene();
                _graphics.ClearScene();
                _graphics.EndScene();
            }) {IsBackground = true};

            _overlayThread.Start();
        }

        public void Stop()
        {
            _isDrawing = false;
        }
    }
}
