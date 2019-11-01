using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WinTiler.LowLevel;
using WinTiler.Overlay;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WinTiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            new Controller(this).SetupKeyboardHooks();

            var ni = new NotifyIcon
            {
                Icon = new Icon("Main.ico"),
                Visible = true,
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Quit", (sender, args) => { Application.Current.Shutdown(); })
                }),
            };
            ni.DoubleClick += (object sender, EventArgs args) => 
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var overlayWindow = new OverlayWindow();
            new Thread(() =>
            {
                overlayWindow.Initialize();
                overlayWindow.Run();
            }){IsBackground = true}.Start();

            new Thread(() =>
            {
                while (true)
                {
                    overlayWindow.Move();
                    Thread.Sleep(100);
                }
            }){IsBackground = true}.Start();
        }
        
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            
            this.Hide();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }
    }
    
    internal class Controller : IDisposable
    {
        private readonly MainWindow _mainWindow;
        private GlobalKeyboardHook _globalKeyboardHook;

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        public Controller(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
//            if (!_registeredKeys.Contains(e.KeyboardData.Key)) 
//                return;
    
            
            if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.SysKeyDown)
                return;

            if (GlobalKeyboardHook.WinAltPressed)
            {
                int virtualScreenWidth = (int) WpfScreen.Primary.WorkingArea.Width;
                int virtualScreenHeight = (int) WpfScreen.Primary.WorkingArea.Height;

                int width = virtualScreenWidth;
                int height = virtualScreenHeight;

                var win = new WindowManipulation();
                if (e.KeyboardData.Key == Keys.I)
                {
                    Debug.WriteLine("Pressed win-alt-k");
                    win.SetForegroundPosSize(width / 2, 0, width / 2, height / 2);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.U)
                {
                    Debug.WriteLine("Pressed win-alt-j");
                    win.SetForegroundPosSize(0, 0, width / 2, height / 2);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.N)
                {
                    Debug.WriteLine("Pressed win-alt-n");
                    win.SetForegroundPosSize(0, height / 2, width / 2, height / 2);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.M)
                {
                    Debug.WriteLine("Pressed win-alt-m");
                    win.SetForegroundPosSize(width / 2, height / 2, width / 2, height / 2);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.J)
                {
                    Debug.WriteLine("Pressed win-alt-j");
                    win.SetForegroundPosSize(width / 4, height / 4, width / 2, height / 2);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.K)
                {
                    Debug.WriteLine("Pressed win-alt-k");
                    win.SetForegroundPosSize(width / 4, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.H)
                {
                    Debug.WriteLine("Pressed win-alt-h");
                    win.SetForegroundPosSize(0, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.L)
                {
                    Debug.WriteLine("Pressed win-alt-l");
                    win.SetForegroundPosSize(width / 2, 0, width / 2, 3 * height / 4);
                    e.Handled = true;
                }
                else if (e.KeyboardData.Key == Keys.Enter)
                {
                    Debug.WriteLine("Pressed win-alt-enter");
                    _mainWindow.Show();
                    e.Handled = true;
                }
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}