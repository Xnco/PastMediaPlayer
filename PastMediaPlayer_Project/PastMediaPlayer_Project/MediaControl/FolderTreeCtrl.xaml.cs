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

            // 通用右键菜单
            ContextMenu menu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Play";
            menuItem.Click += MenuItem_Click;
            menu.Items.Add(menuItem);

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
                    string[] names = root.childs[i].fullPath.Split('.');
                    string name = $"【{names[names.Length - 1]}】 {root.childs[i].name}";
                    item.Header = name;
                    item.ContextMenu = menu;
                }

                item.MouseRightButtonDown += RightSelectedFile;
                item.PreviewMouseLeftButtonDown += SelectedFile;
                root.curItem.Items.Add(item);
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
