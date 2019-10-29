using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PastMediaPlayer_Project.MediaControl;

namespace PastMediaPlayer_Project
{
    class LocalInfo
    {
        public static LocalInfo GetSingle()
        {
            if (instance == null)
            {
                instance = new LocalInfo();
            }
            return instance;
        }

        private static LocalInfo instance;

        private LocalInfo()
        {
            // 我的文档
            configDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\PastMediaPlayer\";
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            folderLocalPath = configDir + "config.txt";
        }

        private readonly string configDir;
        private readonly string folderLocalPath;

        public FolderTree rootFolderTree;

        public string FolderPath {
            get
            {
                string path = "";
                if (!string.IsNullOrEmpty(folderLocalPath) 
                    && Directory.Exists(configDir)
                    && File.Exists(folderLocalPath)
                    )
                    path = File.ReadAllText(folderLocalPath);
                return path;
            }
            set
            {
                if (!string.IsNullOrEmpty(folderLocalPath)
                   && Directory.Exists(configDir)
                   && File.Exists(folderLocalPath)
                   )
                    File.WriteAllText(folderLocalPath, value);
            }
        }

        // 初始化本地 root 数据
        public void InitRoot_FolderTree(string varRootPath)
        {
            if (string.IsNullOrEmpty(varRootPath))
            {
                return;
            }

            rootFolderTree = new FolderTree();
            rootFolderTree.fullPath = varRootPath;
            rootFolderTree.isDirectory = true;

            SearchFileToFolderTree(rootFolderTree);
        }

        public void SearchFileToFolderTree(FolderTree root)
        {
            DirectoryInfo rootInfo = new DirectoryInfo(root.fullPath);

            DirectoryInfo[] dirs = rootInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                FolderTree temp = new FolderTree();
                temp.name = dirs[i].Name;
                temp.fullPath = dirs[i].FullName;
                temp.isDirectory = true;
                temp.parentDir = root;
                SearchFileToFolderTree(temp); 

                root.childs.Add(temp);
            }

            FileInfo[] files = rootInfo.GetFiles("*.mp4");
            for (int i = 0; i < files.Length; i++)
            {
                FolderTree temp = new FolderTree();
                string[] names = files[i].FullName.Split('.');
                temp.name = files[i].Name;
                temp.fullPath = files[i].FullName;
                temp.exName = names[names.Length - 1];
                temp.isDirectory = false;
                temp.parentDir = root;
                root.childs.Add(temp);
            }
        }
    }
}
