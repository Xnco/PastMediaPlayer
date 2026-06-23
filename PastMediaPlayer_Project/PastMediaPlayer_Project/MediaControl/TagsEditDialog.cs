using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PastMediaPlayer_Project.MediaControl
{
    /// <summary>
    /// 标签管理对话框：平铺显示标签按钮（点击确认删除）+ 输入框/添加按钮 + 确定按钮
    /// </summary>
    public class TagsEditDialog : Window
    {
        private readonly WrapPanel _tagsPanel;
        private readonly TextBox _input;
        private readonly List<string> _tags;

        /// <summary>用户编辑后的标签集合（不含重复）</summary>
        public List<string> Tags { get { return _tags; } }

        public TagsEditDialog(IEnumerable<string> initialTags, Window owner)
        {
            Title = "管理标签";
            Width = 480;
            Height = 280;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = owner;

            _tags = new List<string>();
            if (initialTags != null)
            {
                foreach (var t in initialTags)
                {
                    string s = (t ?? string.Empty).Trim();
                    if (s.Length > 0 && !_tags.Contains(s)) _tags.Add(s);
                }
            }

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 第一行：已有标签平铺
            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            _tagsPanel = new WrapPanel { Orientation = Orientation.Horizontal };
            scroll.Content = _tagsPanel;
            Grid.SetRow(scroll, 0);
            grid.Children.Add(scroll);

            // 第二行：输入框 + 添加按钮
            var addPanel = new DockPanel { LastChildFill = true, Margin = new Thickness(0, 8, 0, 0) };
            var addBtn = new Button { Content = "添加", Width = 70, Height = 26, Margin = new Thickness(6, 0, 0, 0) };
            DockPanel.SetDock(addBtn, Dock.Right);
            addPanel.Children.Add(addBtn);

            _input = new TextBox { VerticalContentAlignment = VerticalAlignment.Center, Height = 26 };
            addPanel.Children.Add(_input);
            Grid.SetRow(addPanel, 1);
            grid.Children.Add(addPanel);

            addBtn.Click += (s, e) =>
            {
                string text = (_input.Text ?? string.Empty).Trim();
                if (text.Length == 0) return;
                if (_tags.Contains(text))
                {
                    MessageBox.Show(this, "该标签已存在。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                _tags.Add(text);
                _input.Clear();
                RefreshTagButtons();
                _input.Focus();
            };
            _input.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    // 输入框为空：保留默认行为（触发 IsDefault 的确定按钮）；
                    // 有内容时拦截事件并执行“添加”，避免误触确定按钮关闭对话框。
                    if (string.IsNullOrWhiteSpace(_input.Text)) return;
                    addBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    e.Handled = true;
                }
            };

            // 第三行：确定按钮
            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var ok = new Button { Content = "确定", Width = 70, Height = 26, IsDefault = true };
            ok.Click += (s, e) => { DialogResult = true; Close(); };
            btnPanel.Children.Add(ok);
            Grid.SetRow(btnPanel, 2);
            grid.Children.Add(btnPanel);

            Content = grid;
            RefreshTagButtons();
            Loaded += (s, e) => _input.Focus();

            // 没有“取消”按钮：除非外部主动设置过 DialogResult，否则关闭即视为“确定”，
            // 防止用户点击 X 或按 Esc 时丢失刚添加的标签。
            Closing += (s, e) =>
            {
                if (DialogResult == null) DialogResult = true;
            };
        }

        private void RefreshTagButtons()
        {
            _tagsPanel.Children.Clear();
            for (int i = 0; i < _tags.Count; i++)
            {
                string tag = _tags[i];
                var btn = new Button
                {
                    Content = tag,
                    Margin = new Thickness(0, 0, 6, 6),
                    Padding = new Thickness(8, 2, 8, 2),
                    Tag = tag
                };
                btn.Click += (s, e) =>
                {
                    string t = (string)((Button)s).Tag;
                    if (ConfirmDelete(t))
                    {
                        _tags.Remove(t);
                        RefreshTagButtons();
                    }
                };
                _tagsPanel.Children.Add(btn);
            }
        }

        // 自建确认窗口：WindowStartupLocation=CenterOwner 以保证居中于当前对话框
        private bool ConfirmDelete(string tag)
        {
            var dlg = new Window
            {
                Title = "删除标签",
                Width = 320,
                Height = 140,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ShowInTaskbar = false,
            };

            var grid = new Grid { Margin = new Thickness(12) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tip = new TextBlock
            {
                Text = $"确认删除标签：{tag} ？",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(tip, 0);
            grid.Children.Add(tip);

            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 8, 0, 0),
            };
            var ok = new Button { Content = "确定", Width = 70, Height = 26, IsDefault = true, Margin = new Thickness(0, 0, 6, 0) };
            var cancel = new Button { Content = "取消", Width = 70, Height = 26, IsCancel = true };
            ok.Click += (s, e) => { dlg.DialogResult = true; dlg.Close(); };
            cancel.Click += (s, e) => { dlg.DialogResult = false; dlg.Close(); };
            panel.Children.Add(ok);
            panel.Children.Add(cancel);
            Grid.SetRow(panel, 1);
            grid.Children.Add(panel);

            dlg.Content = grid;
            return dlg.ShowDialog() == true;
        }
    }
}
