using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PastMediaPlayer_Project.MediaControl
{
    /// <summary>
    /// MySlider.xaml 的交互逻辑
    /// </summary>
    public partial class MySlider : Slider
    {
        public MySlider()
        {
            InitializeComponent();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left, e.StylusDevice);
                args.RoutedEvent = e.RoutedEvent;
                args.Source = e.Source;
                args.Handled = e.Handled;
                OnPreviewMouseLeftButtonDown(args);
            }
        }
    }
}
