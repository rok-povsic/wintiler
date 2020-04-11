using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WinTiler.KeyboardShortcuts;
using WinTiler.KeyboardShortcuts.LowLevel;
using WinTiler.Overlay;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
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
        private OverlayWindow _overlayWindow;

        private readonly Label[,] _labels;

        private int _mouseDownRow = -1;
        private int _mouseDownCol = -1;

        private int _keyboardLeft = 0;
        private int _keyboardTop = 0;
        private int _keyboardRight = 0;
        private int _keyboardBottom = 0;

        private readonly WindowManipulation _windowManipulation = new WindowManipulation();

        public MainWindow()
        {
            InitializeComponent();

            new KeyboardHooks(this).Setup();

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

            _labels = new Label[FullScreen.NUM_OF_BOXES, FullScreen.NUM_OF_BOXES];
            for (int i = 0; i < FullScreen.NUM_OF_BOXES; i++)
            {
                for (int j = 0; j < FullScreen.NUM_OF_BOXES; j++)
                {
                    _labels[i, j] = (Label) FindName($"Label{i + 1}{j + 1}");
                }
            }

            DetectWindowSize();
        }

        public void DetectWindowSize()
        {
            WindowManipulation.Rect rect = _windowManipulation.GetForegroundRect();

            _keyboardTop = (int) Math.Round((double) rect.Top / FullScreen.ScreenHeight * FullScreen.NUM_OF_BOXES);
            _keyboardRight = (int) Math.Round((double) rect.Right / FullScreen.ScreenWidth * FullScreen.NUM_OF_BOXES) - 1;
            _keyboardBottom = (int) Math.Round((double) rect.Bottom / FullScreen.ScreenHeight * FullScreen.NUM_OF_BOXES) - 1;
            _keyboardLeft = (int) Math.Round((double) rect.Left / FullScreen.ScreenWidth * FullScreen.NUM_OF_BOXES);

            if (_keyboardLeft < 0) _keyboardLeft = 0;
            if (_keyboardTop < 0) _keyboardTop = 0;
            if (_keyboardRight >= FullScreen.NUM_OF_BOXES) _keyboardRight = FullScreen.NUM_OF_BOXES - 1;
            if (_keyboardBottom >= FullScreen.NUM_OF_BOXES) _keyboardBottom = FullScreen.NUM_OF_BOXES - 1;

            HighlightLabels(_keyboardTop, _keyboardRight, _keyboardBottom, _keyboardLeft);
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
            DrawOverlay(_mouseDownCol, _mouseDownRow, _mouseDownCol, _mouseDownRow);
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Label label = (Label) sender;

                int col = int.Parse(label.Name[5].ToString()) - 1;
                int row = int.Parse(label.Name[6].ToString()) - 1;

                ClearLabels();

                if (_mouseDownRow == -1 || _mouseDownCol == -1 )
                {
                    return;
                }

                int left = LeftHighlightedCol(col);
                int right = RightHighlightedCol(col);
                int top = TopHighlighedRow(row);
                int bottom = BottomHighlightedRow(row);

                Mark(top, right, bottom, left);
            }
        }

        private void Mark(int top, int right, int bottom, int left)
        {
            HighlightLabels(top, right, bottom, left);
            DrawOverlay(top, right, bottom, left);
        }

        private void HighlightLabels(int top, int right, int bottom, int left)
        {
            List<Label> labels = LabelsInArea(top, right, bottom, left);

            ClearLabels();

            foreach (Label foundLabel in labels)
            {
                HighlightLabel(foundLabel);
            }
        }

        private void DrawOverlay(int top, int right, int bottom, int left)
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow();
                _overlayWindow.Initialize();
            }

            _overlayWindow.Run(
                left * FullScreen.BoxWidth,
                top * FullScreen.BoxHeight,
                (right + 1) * FullScreen.BoxWidth,
                (bottom + 1) * FullScreen.BoxHeight
            );
        }

        private void Label_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hide();

            Label label = (Label) sender;

            int currentCol = int.Parse(label.Name[5].ToString()) - 1;
            int currentRow = int.Parse(label.Name[6].ToString()) - 1;

            int leftHighlightedCol = LeftHighlightedCol(currentCol);
            int topHighlightedRow = TopHighlighedRow(currentRow);
            int rightHighlightedCol = RightHighlightedCol(currentCol);
            int bottomHighlightedRow = BottomHighlightedRow(currentRow);

            _windowManipulation.PlaceWindow(
                leftHighlightedCol,
                topHighlightedRow,
                rightHighlightedCol,
                bottomHighlightedRow
            );

            ClearLabels();
            _overlayWindow.Stop();

            _mouseDownRow = -1;
            _mouseDownCol = -1;
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
            for (int i = 0; i < FullScreen.NUM_OF_BOXES; i++)
            {
                for (int j = 0; j < FullScreen.NUM_OF_BOXES; j++)
                {
                    yield return _labels[i, j];
                }
            }
        }

        private List<Label> LabelsInArea(int top, int right, int bottom, int left)
        {
            return GetLabelsInRectangle(top, right, bottom, left);
        }

        private List<Label> GetLabelsInRectangle(int top, int right, int bottom, int left)
        {
            var result = new List<Label>();
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    result.Add((Label) FindName($"Label{i + 1}{j + 1}"));
                }
            }

            return result;
        }

        private int TopHighlighedRow(int currentRow)
        {
            return Math.Min(currentRow, _mouseDownRow);
        }

        private int BottomHighlightedRow(int currentRow)
        {
            return Math.Max(currentRow, _mouseDownRow);
        }

        private int LeftHighlightedCol(int currentCol)
        {
            return Math.Min(currentCol, _mouseDownCol);
        }

        private int RightHighlightedCol(int currentCol)
        {
            return Math.Max(currentCol, _mouseDownCol);
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            bool marking = false;
            if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.Left)
            {
                if (_keyboardRight - _keyboardLeft == 0) return;

                _keyboardRight -= 1;

                marking = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.Right)
            {
                if (_keyboardRight + 1 == FullScreen.NUM_OF_BOXES) return;

                _keyboardRight += 1;

                marking = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.Up)
            {
                if (_keyboardBottom - _keyboardTop == 0) return;

                _keyboardBottom -= 1;

                marking = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.Down)
            {
                if (_keyboardBottom + 1 == FullScreen.NUM_OF_BOXES) return;

                _keyboardBottom += 1;

                marking = true;
            }
            else if (e.Key == Key.Left)
            {
                if (_keyboardLeft == 0) return;

                _keyboardLeft -= 1;
                _keyboardRight -= 1;

                marking = true;
            }
            else if (e.Key == Key.Right)
            {
                if (_keyboardRight + 1 == FullScreen.NUM_OF_BOXES) return;

                _keyboardLeft += 1;
                _keyboardRight += 1;

                marking = true;
            }
            else if (e.Key == Key.Up)
            {
                if (_keyboardTop == 0) return;

                _keyboardTop -= 1;
                _keyboardBottom -= 1;

                marking = true;
            }
            else if (e.Key == Key.Down)
            {
                if (_keyboardBottom + 1 == FullScreen.NUM_OF_BOXES) return;

                _keyboardTop += 1;
                _keyboardBottom += 1;

                marking = true;
            }
            else if (e.Key == Key.Enter)
            {
                Hide();

                _windowManipulation.PlaceWindow(_keyboardLeft, _keyboardTop, _keyboardRight, _keyboardBottom);

                ClearLabels();
                _overlayWindow.Stop();
            }

            if (marking)
            {
                Mark(_keyboardTop, _keyboardRight, _keyboardBottom, _keyboardLeft);
            }
        }
    }
}
