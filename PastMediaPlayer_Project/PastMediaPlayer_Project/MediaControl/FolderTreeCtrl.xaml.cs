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
            rootPath = "";

            configDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\PastMediaPlayer\";
            configPath = configDir + "config.txt";

            if (File.Exists(configPath))
            {
                InitFolderTree(File.ReadAllText(configPath));
            }
        }

        // Local Config
        public string configDir;
        public string configPath; 

        public string rootPath;
        public event Action<string> selectedMediaFile;

        public void InitFolderTree(string varRootPath)
        {
            rootPath = varRootPath;
            if (!string.IsNullOrEmpty(rootPath))
            {
                SearchFileToTree(RootNode, rootPath);
                RootNode.IsExpanded = true;

                // Local
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                File.WriteAllText(configPath, rootPath);
            }
        }

        public void SearchFileToTree(TreeViewItem root, string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            DirectoryInfo rootInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirs = rootInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                TreeViewItem tree = new TreeViewItem();
                tree.Header = dirs[i].Name;
                root.Items.Add(tree);
                SearchFileToTree(tree, dirs[i].FullName);
            }

            FileInfo[] files = rootInfo.GetFiles("*.mp4");
            for (int i = 0; i < files.Length; i++)
            {
                TreeViewItem tree = new TreeViewItem();
                string[] names = files[i].FullName.Split('.');
                string name = $"【{names[names.Length -1 ]}】 {files[i].Name}";
                tree.Header = name;
                tree.DataContext = files[i].FullName;
                tree.Selected += SelectedFile;
                root.Items.Add(tree);
            }
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SelectedFile(object sender, RoutedEventArgs args)
        {
            if (selectedMediaFile != null)
            {
               
                TreeViewItem s = sender as TreeViewItem;
                if (s != null)
                {
                    selectedMediaFile((string)s.DataContext);
                    //string name = s.Header as string;
                    //string[] infos = name.Split("..");
                    //if (infos.Length > 1)
                    //{
                    //    selectedMediaFile(infos[2]);
                    //}
                }
            }
        }
    }
}
