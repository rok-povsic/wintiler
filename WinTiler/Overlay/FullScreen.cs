using System.Windows.Forms;

namespace WinTiler.Overlay
{
    public class FullScreen
    {
        private static int NUM_OF_BOXES = 4;

        public static int ScreenWidth => Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight => Screen.PrimaryScreen.Bounds.Height;

        public static int BoxWidth => ScreenWidth / NUM_OF_BOXES;
        public static int BoxHeight => ScreenHeight / NUM_OF_BOXES;
    }
}