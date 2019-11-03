using System;
using System.Collections.Generic;
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
using Brushes = System.Windows.Media.Brushes;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace WinTiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Thread _overlayThread;
        private OverlayWindow _overlayWindow;

        private readonly Label[,] _labels;

        private int _mouseDownRow = -1;
        private int _mouseDownCol = -1;

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

            _labels = new Label[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _labels[i, j] = (Label) FindName($"Label{i + 1}{j + 1}");
                }
            }
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

        private void Label_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label) sender;

            _mouseDownCol = int.Parse(label.Name[5].ToString()) - 1;
            _mouseDownRow = int.Parse(label.Name[6].ToString()) - 1;

            HighlightLabel(label);
        }

        private void Label_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearLabels();

            _mouseDownRow = -1;
            _mouseDownCol = -1;
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label label = (Label) sender;

                int col = int.Parse(label.Name[5].ToString()) - 1;
                int row = int.Parse(label.Name[6].ToString()) - 1;

                ClearLabels();

                foreach (Label foundLabel in LabelsInArea(row, col))
                {
                    HighlightLabel(foundLabel);
                }
            }
        }

        private void ClearLabels()
        {
            foreach (Label label in AllLabels())
            {
                ClearLabel(label);
            }
        }

        private void HighlightLabel(Label label)
        {
            label.Background = Brushes.Coral;
        }

        private void ClearLabel(Label label)
        {
            label.Background = Brushes.LightGray;
        }

        private IEnumerable<Label> AllLabels()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    yield return _labels[i, j];
                }
            }
        }

        private List<Label> LabelsInArea(int currentRow, int currentCol)
        {
            var result = new List<Label>();
            if (_mouseDownRow == -1 || _mouseDownCol == -1 )
            {
                return result;
            }

            int topRow = Math.Min(currentRow, _mouseDownRow);
            int bottomRow = Math.Max(currentRow, _mouseDownRow);

            int leftCol = Math.Min(currentCol, _mouseDownCol);
            int rightCol = Math.Max(currentCol, _mouseDownCol);

            for (int i = leftCol; i <= rightCol; i++)
            {
                for (int j = topRow; j <= bottomRow; j++)
                {
                    result.Add((Label) FindName($"Label{i + 1}{j + 1}"));
                }
            }

            return result;
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