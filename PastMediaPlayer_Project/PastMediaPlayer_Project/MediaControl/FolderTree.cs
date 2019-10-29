using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace PastMediaPlayer_Project.MediaControl
{
    /// <summary>
    /// 路径多叉树
    /// </summary>
    public class FolderTree : INotifyPropertyChanged
    {
        public int id;
        public string name;
        public string fullPath;
        public string exName;
        public string iconPath = "Images/FolderIcon.png"; // 默认文件夹图标

        public bool isDirectory; // 是否是文件夹
        public bool isExpanded; // 是否展开

        public FolderTree parentDir; // 父文件夹 

        public List<string> tags; // 标签
        public List<FolderTree> childs; 

        public TreeViewItem curItem;

        public event PropertyChangedEventHandler PropertyChanged;

        public FolderTree()
        {
            isExpanded = true;
            tags = new List<string>();
            childs = new List<FolderTree>();
        }
    }
}
