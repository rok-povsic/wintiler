using System;
using System.Runtime.InteropServices;
using WinTiler.Overlay;

namespace WinTiler.KeyboardShortcuts.LowLevel
{
    public class WindowManipulation
    {
        /**
         * Place the window to the appropriate box-location. Appropriately handles maximizing/restoring the window.
         */
        public void PlaceWindow(int left, int top, int right, int bottom)
        {
            if (
                left == 0 &&
                top == 0 &&
                right == FullScreen.NUM_OF_BOXES - 1 &&
                bottom == FullScreen.NUM_OF_BOXES - 1
            )
            {
                Maximize();
            }
            else
            {
                Restore();
                SetForegroundPos(
                    left * FullScreen.BoxWidth,
                    top * FullScreen.BoxHeight,
                    (right + 1) * FullScreen.BoxWidth,
                    (bottom + 1) * FullScreen.BoxHeight
                );
            }
        }

        public Rect GetForegroundRect()
        {
            var rct = new Rect();
            WindowsApi.GetWindowRect(WindowsApi.GetForegroundWindow(), ref rct);
            return rct;
        }

        private void SetForegroundPos(int left, int top, int right, int bottom)
        {
            int w = right - left;
            int h = bottom - top;
            WindowsApi.SetWindowPos(WindowsApi.GetForegroundWindow(), IntPtr.Zero, left, top, w, h, 4);
        }

        /**
         * Maximizes the window.
         */
        private void Maximize()
        {
            WindowsApi.ShowWindow(WindowsApi.GetForegroundWindow(), WindowsApi.SW_MAXIMIZE);
        }

        /**
         * If the window is maximized, it restores it to the previous position.
         */
        private void Restore()
        {
            WindowsApi.ShowWindow(WindowsApi.GetForegroundWindow(), WindowsApi.SW_RESTORE);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static class WindowsApi
        {
            // All available ShowWindow commands:
            // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
            internal const int SW_MAXIMIZE = 3;
            internal const int SW_RESTORE = 9;

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            internal static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            internal static extern bool SetWindowPos(
                IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags
            );

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
        }
    }
}
