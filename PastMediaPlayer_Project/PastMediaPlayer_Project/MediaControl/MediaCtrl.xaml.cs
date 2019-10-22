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
    /// MediaCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaCtrl : UserControl
    {
        public MediaCtrl()
        {
            InitializeComponent();
        }

        public void ControllingMediaSound(double value)
        {
            MainMedia.Volume += value;
        }

        public void OpenMediaByUrl(string path)
        {
            MainMedia.Source = new Uri(path);
        }

        // 媒体打开后
        private void MainMedia_MediaOpened(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"打开 {MainMedia.Source.AbsolutePath} 成功");
        }
        
        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMedia.Source == null) return;

            MainMedia.Position += TimeSpan.FromSeconds(-5);
        }

        private void Play_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMedia.Source == null) return; 

            if (MainMedia.Clock.IsPaused)
            {
                MainMedia.Play();
            }
            else
            {
                MainMedia.Pause();
            }
        }

        private void Fore_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMedia.Source == null) return;

            MainMedia.Position += TimeSpan.FromSeconds(5);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainMedia.Source == null) return;

            MainMedia.Position = TimeSpan.FromSeconds(e.NewValue);
        }
    }
}
