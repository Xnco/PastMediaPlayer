using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PastMediaPlayer_Project.MediaControl
{
    /// <summary>
    /// 评分输入对话框：标题"修改评分："+输入框 + 确认/取消
    /// 评分范围 0.1-10（最多一位小数）；输入为空或 0 表示清除评分
    /// </summary>
    public class RatingInputDialog : Window
    {
        private readonly TextBox _input;

        /// <summary>用户输入的评分。null 表示清除评分</summary>
        public double? Rating { get; private set; }

        public RatingInputDialog(double? initialRating, Window owner)
        {
            Title = "修改评分";
            Width = 400;
            Height = 140;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = owner;

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // 第一行：标签 + 输入框
            var topPanel = new DockPanel { LastChildFill = true };
            var label = new TextBlock
            {
                Text = "修改评分：",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };
            DockPanel.SetDock(label, Dock.Left);
            topPanel.Children.Add(label);

            _input = new TextBox
            {
                Text = initialRating.HasValue ? initialRating.Value.ToString("0.#", CultureInfo.InvariantCulture) : string.Empty,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 26
            };
            topPanel.Children.Add(_input);
            Grid.SetRow(topPanel, 0);
            grid.Children.Add(topPanel);

            // 第二行：按钮
            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var ok = new Button { Content = "确认", Width = 70, Height = 26, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var cancel = new Button { Content = "取消", Width = 70, Height = 26, IsCancel = true };
            ok.Click += (s, e) =>
            {
                string text = (_input.Text ?? string.Empty).Trim();
                if (text.Length == 0)
                {
                    // 空输入：清除评分
                    Rating = null;
                    DialogResult = true;
                    Close();
                    return;
                }

                double value;
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) || value < 0 || value > 10)
                {
                    MessageBox.Show(this, "请输入 0-10 之间的评分（最低 0.1，输入 0 表示清除评分）。", "评分无效",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _input.Focus();
                    _input.SelectAll();
                    return;
                }

                // 限制到一位小数
                value = System.Math.Round(value, 1);

                // 0 视为清除评分；非 0 值最低 0.1
                if (value == 0)
                {
                    Rating = null;
                }
                else
                {
                    if (value < 0.1) value = 0.1;
                    Rating = value;
                }
                DialogResult = true;
                Close();
            };
            btnPanel.Children.Add(ok);
            btnPanel.Children.Add(cancel);
            Grid.SetRow(btnPanel, 1);
            grid.Children.Add(btnPanel);

            Content = grid;

            Loaded += (s, e) => { _input.Focus(); _input.SelectAll(); };
        }
    }
}
