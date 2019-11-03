using System;
using System.Runtime.InteropServices;
using System.Text;

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
        
        public static bool SetHwndPos(IntPtr hwnd, int x, int y)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, 5);
        }

        public static bool SetHwndSize(IntPtr hwnd, int w, int h)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, 0, 0, w, h, 6);
        }
        
        public static bool SetHwndPosSize(IntPtr hwnd, int x, int y, int w, int h)
        {
            return SetWindowPos(hwnd, IntPtr.Zero, x, y, w, h, 4);
        }

        public void SetForegroundPos(int x, int y)
        {
            SetHwndPos(GetForegroundWindow(), x, y);
        }

        public void SetForegroundPos(int left, int top, int right, int bottom)
        {
            SetHwndPosSize(GetForegroundWindow(), left, top, right - left, bottom - top);
        }

        public void SetForegroundSize(int x, int y)
        {
            SetHwndSize(GetForegroundWindow(), x, y);
        }

        public void SetForegroundPosSize(int x, int y, int sizeX, int sizeY)
        {
            SetHwndPosSize(GetForegroundWindow(), x, y, sizeX, sizeY);
        }
    }
}