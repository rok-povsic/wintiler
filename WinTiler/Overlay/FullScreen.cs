using System.Windows;

namespace WinTiler.Overlay
{
    public class FullScreen
    {
        private static int NUM_OF_BOXES = 4;

        public static int ScreenWidth => (int)SystemParameters.WorkArea.Width;
        public static int ScreenHeight => (int)SystemParameters.WorkArea.Height;

        public static int BoxWidth => ScreenWidth / NUM_OF_BOXES;
        public static int BoxHeight => ScreenHeight / NUM_OF_BOXES;
    }
}