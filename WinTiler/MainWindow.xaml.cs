using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            DrawOverlay(_mouseDownCol, _mouseDownRow);
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

                DrawOverlay(col, row);
            }
        }

        private void DrawOverlay(int currentCol, int currentRow)
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow();
                _overlayWindow.Initialize();
            }

            _overlayWindow.Run(
                LeftHighlightedCol(currentCol) * FullScreen.BoxWidth,
                TopHighlighedRow(currentRow) * FullScreen.BoxHeight,
                (RightHighlightedCol(currentCol) + 1) * FullScreen.BoxWidth,
                (BottomHighlightedRow(currentRow) + 1) * FullScreen.BoxHeight
            );
        }

        private void Label_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hide();

            Label label = (Label) sender;

            int currentCol = int.Parse(label.Name[5].ToString()) - 1;
            int currentRow = int.Parse(label.Name[6].ToString()) - 1;

            new WindowManipulation().SetForegroundPos(
                LeftHighlightedCol(currentCol) * FullScreen.BoxWidth,
                TopHighlighedRow(currentRow) * FullScreen.BoxHeight,
                (RightHighlightedCol(currentCol) + 1) * FullScreen.BoxWidth,
                (BottomHighlightedRow(currentRow) + 1) * FullScreen.BoxHeight
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

            for (int i = LeftHighlightedCol(currentCol); i <= RightHighlightedCol(currentCol); i++)
            {
                for (int j = TopHighlighedRow(currentRow); j <= BottomHighlightedRow(currentRow); j++)
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
    }
}
