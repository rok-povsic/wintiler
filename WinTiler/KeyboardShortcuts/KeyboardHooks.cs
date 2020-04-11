using System;
using System.Windows.Forms;
using WinTiler.KeyboardShortcuts.LowLevel;
using WinTiler.Overlay;

namespace WinTiler.KeyboardShortcuts
{
    public class KeyboardHooks : IDisposable
    {
        private readonly MainWindow _mainWindow;
        private GlobalKeyboardHook _globalKeyboardHook;

        public void Setup()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        public KeyboardHooks(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (!(e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown && GlobalKeyboardHook.WinAltPressed))
                return;

            int width = FullScreen.ScreenWidth;
            int height = FullScreen.ScreenHeight;

            var win = new WindowManipulation();
            switch (e.KeyboardData.Key)
            {
                case Keys.I:
                {
                    win.SetForegroundPosSize(width / 2, 0, width / 2, height / 2);
                    e.Handled = true;
                    break;
                }
                case Keys.U:
                {
                    win.SetForegroundPosSize(0, 0, width / 2, height / 2);
                    e.Handled = true;
                    break;
                }
                case Keys.N:
                {
                    win.SetForegroundPosSize(0, height / 2, width / 2, height / 2);
                    e.Handled = true;
                    break;
                }
                case Keys.M:
                {
                    win.SetForegroundPosSize(width / 2, height / 2, width / 2, height / 2);
                    e.Handled = true;
                    break;
                }
                case Keys.J:
                {
                    win.SetForegroundPosSize(width / 4, height / 4, width / 2, height / 2);
                    e.Handled = true;
                    break;
                }
                case Keys.K:
                {
                    win.SetForegroundPosSize(width / 4, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                    break;
                }
                case Keys.H:
                {
                    win.SetForegroundPosSize(0, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                    break;
                }
                case Keys.L:
                {
                    win.SetForegroundPosSize(width / 2, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                    break;
                }
                case Keys.Enter:
                {
                    _mainWindow.Show();
                    e.Handled = true;
                    break;
                }
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}
