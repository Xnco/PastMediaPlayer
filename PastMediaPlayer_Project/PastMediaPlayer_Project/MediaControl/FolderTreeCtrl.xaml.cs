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

            rootTreeViewItem.IsExpanded = true;
            TreeViewRoot.Items.Add(rootTreeViewItem);
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

        // 文件节点的显示文本：有别名则附加别名
        private string BuildFileHeader(FolderTree tree)
        {
            string[] names = tree.fullPath.Split('.');
            string baseName = $"【{names[names.Length - 1]}】 {tree.name}";
            string alias = TryGetAlias(tree);
            return string.IsNullOrEmpty(alias) ? baseName : $"{baseName}  ({alias})";
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
    }
}
