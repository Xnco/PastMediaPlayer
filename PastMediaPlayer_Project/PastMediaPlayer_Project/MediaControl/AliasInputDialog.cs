using System.Windows;
using System.Windows.Controls;

namespace PastMediaPlayer_Project.MediaControl
{
    /// <summary>
    /// 别名输入对话框：标题"修改别名："+输入框 + 确认/取消
    /// </summary>
    public class AliasInputDialog : Window
    {
        private readonly TextBox _input;
        public string AliasText { get { return _input.Text; } }

        public AliasInputDialog(string initialAlias, Window owner)
        {
            Title = "修改别名";
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
                Text = "修改别名：",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };
            DockPanel.SetDock(label, Dock.Left);
            topPanel.Children.Add(label);

            _input = new TextBox
            {
                Text = initialAlias ?? string.Empty,
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
            ok.Click += (s, e) => { DialogResult = true; Close(); };
            btnPanel.Children.Add(ok);
            btnPanel.Children.Add(cancel);
            Grid.SetRow(btnPanel, 1);
            grid.Children.Add(btnPanel);

            Content = grid;

            Loaded += (s, e) => { _input.Focus(); _input.SelectAll(); };
        }
    }
}
