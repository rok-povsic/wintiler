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

            var win = new WindowManipulation();
            switch (e.KeyboardData.Key)
            {
                case Keys.I:
                {
                    win.PlaceWindow(2, 0, 3, 1);
                    e.Handled = true;
                    break;
                }
                case Keys.U:
                {
                    win.PlaceWindow(0, 0, 1, 1);
                    e.Handled = true;
                    break;
                }
                case Keys.N:
                {
                    win.PlaceWindow(0, 2, 1, 3);
                    e.Handled = true;
                    break;
                }
                case Keys.M:
                {
                    win.PlaceWindow(2, 2, 3, 3);
                    e.Handled = true;
                    break;
                }
                case Keys.J:
                {
                    win.PlaceWindow(1, 1, 2, 2);
                    e.Handled = true;
                    break;
                }
                case Keys.K:
                {
                    win.PlaceWindow(1, 0, 2, 2);
                    e.Handled = true;
                    break;
                }
                case Keys.H:
                {
                    win.PlaceWindow(0, 0, 2, 2);
                    e.Handled = true;
                    break;
                }
                case Keys.L:
                {
                    win.PlaceWindow(2, 0, 3, 2);
                    e.Handled = true;
                    break;
                }
                case Keys.Enter:
                {
                    _mainWindow.DetectWindowSize();
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
