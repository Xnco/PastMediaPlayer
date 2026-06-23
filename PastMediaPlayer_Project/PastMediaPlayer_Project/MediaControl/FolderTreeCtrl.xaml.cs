using System;
using System.IO;
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
    /// FolderTreeCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class FolderTreeCtrl : UserControl
    {
        public FolderTreeCtrl()
        {
            InitializeComponent();
        }

        public TreeViewItem rootTreeViewItem;

        public event Action<FolderTree> selectedMediaFile;

        public void InitRootTreeItem(FolderTree root)
        {
            if (root == null || string.IsNullOrEmpty(root.fullPath))
            {
                return;
            }

            if (rootTreeViewItem != null)
            {
                TreeViewRoot.Items.Remove(rootTreeViewItem);
                rootTreeViewItem = null;
            }

            // 记录现在打开的 path
            LocalInfo.GetSingle().FolderPath = root.fullPath;

            // root
            rootTreeViewItem = new TreeViewItem();
            rootTreeViewItem.Header = root.fullPath;
            rootTreeViewItem.DataContext = root;
            root.curItem = rootTreeViewItem;

            CreateItemByFolderInfo(root);
            // 初次构建后，隐藏没有任何受支持视频的文件夹
            ApplyFilter(root, string.Empty);

            rootTreeViewItem.IsExpanded = true;
            TreeViewRoot.Items.Add(rootTreeViewItem);

            RefreshTagList();
        }

        public void CreateItemByFolderInfo(FolderTree root)
        {
            if (string.IsNullOrEmpty(root.fullPath)) return;

            for (int i = 0; i < root.childs.Count; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.DataContext = root.childs[i];
                root.childs[i].curItem = item;

                if (root.childs[i].isDirectory)
                {
                    // directory
                    TreeViewHeader header = new TreeViewHeader();
                    header.SetImagePath(root.childs[i].iconPath);
                    header.SetText(root.childs[i].name);
                    item.Header = header;
                    CreateItemByFolderInfo(root.childs[i]);
                }
                else
                {
                    // file
                    item.Header = BuildFileHeader(root.childs[i]);
                    item.ContextMenu = BuildFileContextMenu(root.childs[i]);
                }

                item.MouseRightButtonDown += RightSelectedFile;
                item.PreviewMouseLeftButtonDown += SelectedFile;
                root.curItem.Items.Add(item);
            }
        }

        // 文件节点的显示文本：有别名时把别名前置为【别名】文件名；按评分着色
        private object BuildFileHeader(FolderTree tree)
        {
            string alias = TryGetAlias(tree);
            string text = string.IsNullOrEmpty(alias) ? tree.name : $"【{alias}】{tree.name}";

            TextBlock tb = new TextBlock { Text = text };
            double? rating = TryGetRating(tree);
            if (rating.HasValue)
            {
                /*
                 绿色系：Green（深绿 #008000）、DarkGreen（更深 #006400）、ForestGreen、SeaGreen、LimeGreen、Lime（亮绿 #00FF00）、OliveDrab、MediumSeaGreen
                红色系：Red、DarkRed、Crimson、IndianRed、Firebrick
                蓝色系：Blue、DarkBlue、SteelBlue、DodgerBlue、RoyalBlue
                灰色系：Gray、DarkGray、LightGray、DimGray、Silver
                其它：Orange、Gold、Yellow、Purple、Pink、Brown、Black、White...
                 */

                Color color;
                if (rating.Value > 8) color = Colors.Blue;
                else if (rating.Value >= 6) color = Colors.Green;
                else color = Colors.Gray;
                tb.Foreground = new SolidColorBrush(color);
            }
            return tb;
        }

        // 按文件独立创建右键菜单（菜单项文本需要随别名变化）
        private ContextMenu BuildFileContextMenu(FolderTree tree)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem playItem = new MenuItem();
            playItem.Header = "Play";
            playItem.Click += MenuItem_Click;
            menu.Items.Add(playItem);

            MenuItem aliasItem = new MenuItem();
            string alias = TryGetAlias(tree);
            aliasItem.Header = string.IsNullOrEmpty(alias) ? "新增别名" : alias;
            aliasItem.Tag = tree;
            aliasItem.Click += AliasMenuItem_Click;
            menu.Items.Add(aliasItem);

            MenuItem ratingItem = new MenuItem();
            double? rating = TryGetRating(tree);
            ratingItem.Header = rating.HasValue
                ? $"评分：{rating.Value.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)}"
                : "新增评分";
            ratingItem.Tag = tree;
            ratingItem.Click += RatingMenuItem_Click;
            menu.Items.Add(ratingItem);

            MenuItem tagsItem = new MenuItem();
            var tags = TryGetTags(tree);
            tagsItem.Header = (tags != null && tags.Count > 0) ? string.Join(";", tags) : "新增标签";
            tagsItem.Tag = tree;
            tagsItem.Click += TagsMenuItem_Click;
            menu.Items.Add(tagsItem);

            return menu;
        }

        private static string TryGetAlias(FolderTree tree)
        {
            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null || tree == null || string.IsNullOrEmpty(tree.fullPath)) return null;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return null;
            var entry = ws.Cache.Get(key);
            return entry?.Alias;
        }

        private static double? TryGetRating(FolderTree tree)
        {
            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null || tree == null || string.IsNullOrEmpty(tree.fullPath)) return null;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return null;
            var entry = ws.Cache.Get(key);
            return entry?.Rating;
        }

        private static List<string> TryGetTags(FolderTree tree)
        {
            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null || tree == null || string.IsNullOrEmpty(tree.fullPath)) return null;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return null;
            var entry = ws.Cache.Get(key);
            return entry?.Tags;
        }

        private void AliasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            FolderTree tree = mi?.Tag as FolderTree;
            if (tree == null) return;

            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null) return;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return;

            string current = ws.Cache.Get(key)?.Alias ?? string.Empty;
            var dlg = new AliasInputDialog(current, Window.GetWindow(this));
            if (dlg.ShowDialog() == true)
            {
                string newAlias = (dlg.AliasText ?? string.Empty).Trim();
                var entry = ws.Cache.GetOrCreate(key);
                entry.Alias = string.IsNullOrEmpty(newAlias) ? null : newAlias;
                ws.Cache.Save();

                // 刷新 UI：文件节点标题 + 右键菜单里的别名项
                if (tree.curItem != null)
                {
                    tree.curItem.Header = BuildFileHeader(tree);
                    tree.curItem.ContextMenu = BuildFileContextMenu(tree);
                }
            }
        }

        private void RatingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            FolderTree tree = mi?.Tag as FolderTree;
            if (tree == null) return;

            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null) return;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return;

            double? current = ws.Cache.Get(key)?.Rating;
            var dlg = new RatingInputDialog(current, Window.GetWindow(this));
            if (dlg.ShowDialog() == true)
            {
                var entry = ws.Cache.GetOrCreate(key);
                entry.Rating = dlg.Rating;
                ws.Cache.Save();

                // 刷新右键菜单以更新评分显示，并刷新文件标题以应用颜色
                if (tree.curItem != null)
                {
                    tree.curItem.Header = BuildFileHeader(tree);
                    tree.curItem.ContextMenu = BuildFileContextMenu(tree);
                }
            }
        }

        private void TagsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            FolderTree tree = mi?.Tag as FolderTree;
            if (tree == null) return;

            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null) return;
            string key = ws.Cache.MakeKey(tree.fullPath);
            if (string.IsNullOrEmpty(key)) return;

            var current = ws.Cache.Get(key)?.Tags;
            var dlg = new TagsEditDialog(current, Window.GetWindow(this));
            if (dlg.ShowDialog() == true)
            {
                var entry = ws.Cache.GetOrCreate(key);
                entry.Tags = dlg.Tags ?? new List<string>();
                ws.Cache.Save();

                if (tree.curItem != null)
                {
                    tree.curItem.ContextMenu = BuildFileContextMenu(tree);
                }

                RefreshTagList();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                if (TreeViewRoot.SelectedItem != null)
                {
                    TreeViewItem s = TreeViewRoot.SelectedItem as TreeViewItem;
                    if (s != null && s.DataContext != null)
                    {
                        FolderTree tree = s.DataContext as FolderTree;
                        if (tree != null)
                        {
                            selectedMediaFile(tree);
                        }
                    }
                }
            }
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RightSelectedFile(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }

        private void SelectedFile(object sender, RoutedEventArgs args)
        {
            if (selectedMediaFile != null && sender != null)
            {
                TreeViewItem s = sender as TreeViewItem;
                if (s != null && s.DataContext != null)
                {
                    FolderTree tree = s.DataContext as FolderTree;
                    if (tree != null)
                    {
                        selectedMediaFile(tree);
                    }
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchBox != null ? (SearchBox.Text ?? string.Empty).Trim() : string.Empty;
            if (rootTreeViewItem == null) return;

            FolderTree root = rootTreeViewItem.DataContext as FolderTree;
            if (root == null) return;

            // 只过滤文件节点，文件夹一律保持可见
            ApplyFilter(root, keyword);
        }

        // 对文件节点按关键字进行可见性过滤；文件夹在其后代无可见文件时隐藏。
        // 返回值：以 tree 为根的子树中是否存在可见文件
        private bool ApplyFilter(FolderTree tree, string keyword)
        {
            if (tree == null) return false;

            bool hasVisibleFile = false;
            for (int i = 0; i < tree.childs.Count; i++)
            {
                FolderTree child = tree.childs[i];
                if (child.curItem == null) continue;

                if (child.isDirectory)
                {
                    bool subHas = ApplyFilter(child, keyword);
                    child.curItem.Visibility = subHas ? Visibility.Visible : Visibility.Collapsed;
                    if (subHas) hasVisibleFile = true;
                }
                else
                {
                    bool match = IsFileMatch(child, keyword);
                    child.curItem.Visibility = match ? Visibility.Visible : Visibility.Collapsed;
                    if (match) hasVisibleFile = true;
                }
            }
            return hasVisibleFile;
        }

        // 文件名或别名命中（不区分大小写、包含匹配）
        private bool IsFileMatch(FolderTree file, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return true;

            string name = file.name ?? string.Empty;
            if (name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) return true;

            string alias = TryGetAlias(file);
            if (!string.IsNullOrEmpty(alias) &&
                alias.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) return true;

            return false;
        }

        // 刷新右下方标签列表：显示所有标签及其视频数量
        private void RefreshTagList()
        {
            if (TagListView == null) return;

            var ws = LocalInfo.GetSingle().CurrentWorkspace;
            if (ws == null)
            {
                TagListView.ItemsSource = null;
                return;
            }

            var items = new List<object>();
            var tags = ws.Cache.TagIndex?.Tags;
            if (tags != null)
            {
                var list = new List<TagIndexEntry>(tags.Values);
                list.Sort((a, b) =>
                {
                    if (a.Count != b.Count) return b.Count.CompareTo(a.Count);
                    return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                });
                foreach (var ti in list)
                {
                    items.Add(new { Display = $"{ti.Name} : {ti.Count}" });
                }
            }
            TagListView.ItemsSource = items;
        }
    }
}
