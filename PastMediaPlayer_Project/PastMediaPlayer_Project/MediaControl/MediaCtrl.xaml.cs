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
using System.Windows.Threading;

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

            isPlaying = false;
            UriCache = new Dictionary<string, Uri>();

            updateCurTimer = new DispatcherTimer();
            updateCurTimer.Tick += UpdateCurPlayTime;
            updateCurTimer.Interval = TimeSpan.FromSeconds(0.5);
        }

        public bool isPlaying;

        private Dictionary<string, Uri> UriCache;

        private DispatcherTimer updateCurTimer;

        public void ControllingMediaSound(double value)
        {
            MainMedia.Volume += value;
        }

        public void OpenMediaByUrl(string path)
        {
            if (UriCache.ContainsKey(path))
            {
                MainMedia.Source = UriCache[path];
            }
            else
            {
                MainMedia.Source = new Uri(path);
            }
        }

        private void MainMedia_Loaded(object sender, RoutedEventArgs e)
        {
            // 默认播放状态
            MainMedia.Play();
            isPlaying = true;
        }

        private void MainMedia_MediaOpened(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"打开 {MainMedia.Source.AbsolutePath} 成功");

            LastTime.Text = MainMedia.NaturalDuration.TimeSpan.ToString();
            // 新打开一个视频也自动播放
            MainMedia.Play();
            isPlaying = true;

            updateCurTimer.Start();
        }

        private void MainMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            updateCurTimer.Stop();
        }

        private void MainMedia_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            isPlaying = false;
            updateCurTimer.Stop();
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMedia.Source == null) return;

            MainMedia.Position += TimeSpan.FromSeconds(-5);
        }

        private void Play_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMedia.Source == null) return;

            if (isPlaying)
            {
                MainMedia.Pause();
                updateCurTimer.Stop();
            }
            else
            {
                MainMedia.Play();
                updateCurTimer.Start();
            }
            isPlaying = !isPlaying;
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

        private void UpdateCurPlayTime(object sender, EventArgs args)
        {
            if (MainMedia.Source == null || !isPlaying)
            {
                return;
            }

            CurTime.Text = $"{MainMedia.Position.Hours:00}:{MainMedia.Position.Minutes:00}:{MainMedia.Position.Seconds:00}";
        }
    }
}
