using System.Windows;
using WinTiler.KeyboardShortcuts.LowLevel;

namespace WinTiler.Overlay
{
    public class FullScreen
    {
        internal static int NUM_OF_BOXES = 4;

        public static int ScreenWidth => (int)WpfScreen.Primary.WorkingArea.Width;
        public static int ScreenHeight => (int)WpfScreen.Primary.WorkingArea.Height;

        public static int BoxWidth => ScreenWidth / NUM_OF_BOXES;
        public static int BoxHeight => ScreenHeight / NUM_OF_BOXES;
    }
}