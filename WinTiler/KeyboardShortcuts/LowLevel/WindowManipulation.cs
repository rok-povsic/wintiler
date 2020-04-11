using System;
using System.Runtime.InteropServices;
using System.Text;
using WinTiler.Overlay;

namespace WinTiler.KeyboardShortcuts.LowLevel
{
    public class WindowManipulation
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Other commands: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
        private const int SW_MAXIMIZE = 3;
        private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static bool SetHwndPos(IntPtr hwnd, int x, int y)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, 5);
        }

        public static bool SetHwndSize(IntPtr hwnd, int w, int h)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, 0, 0, w, h, 6);
        }

        private static bool SetHwndPosSize(IntPtr hwnd, int x, int y, int w, int h)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, x, y, w, h, 4);
        }

        private void SetForegroundPos(int left, int top, int right, int bottom)
        {
            SetHwndPosSize(GetForegroundWindow(), left, top, right - left, bottom - top);
        }

        public Rect GetForegroundRect()
        {
            var rct = new Rect();
            GetWindowRect(GetForegroundWindow(), ref rct);
            return rct;
        }

        /**
         * Maximizes the window.
         */
        private void Maximize()
        {
            ShowWindow(GetForegroundWindow(), SW_MAXIMIZE);
        }

        /**
         * If the window is maximized, it restores it to the previous position.
         */
        private void Restore()
        {
            ShowWindow(GetForegroundWindow(), SW_RESTORE);
        }

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
    }
}
