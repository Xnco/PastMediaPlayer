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
            configIniPath = configDir + "config.ini";
        }

        private const string KeyFolderPath = "FolderPath";

        private readonly string configDir;
        private readonly string configIniPath;

        // 读取 ini 内容为字典
        private Dictionary<string, string> ReadIni()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(configIniPath)) return dict;

            foreach (string raw in File.ReadAllLines(configIniPath))
            {
                string line = raw?.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("[")) continue;
                int idx = line.IndexOf('=');
                if (idx <= 0) continue;
                string k = line.Substring(0, idx).Trim();
                string v = line.Substring(idx + 1).Trim();
                if (k.Length > 0) dict[k] = v;
            }
            return dict;
        }

        // 写入字典到 ini
        private void WriteIni(Dictionary<string, string> dict)
        {
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            var sb = new StringBuilder();
            foreach (var kv in dict)
            {
                sb.Append(kv.Key).Append('=').Append(kv.Value ?? string.Empty).Append('\n');
            }
            File.WriteAllText(configIniPath, sb.ToString());
        }

        private string GetConfig(string key)
        {
            var dict = ReadIni();
            return dict.TryGetValue(key, out var v) ? v : string.Empty;
        }

        private void SetConfig(string key, string value)
        {
            var dict = ReadIni();
            dict[key] = value ?? string.Empty;
            WriteIni(dict);
        }

        public FolderTree rootFolderTree;

        /// <summary>
        /// 当前打开的工作区。打开新文件夹时会重新创建。
        /// </summary>
        public Workspace CurrentWorkspace { get; private set; }

        public string FolderPath {
            get { return GetConfig(KeyFolderPath); }
            set { SetConfig(KeyFolderPath, value); }
        }

        // 初始化本地 root 数据
        public void InitRoot_FolderTree(string varRootPath)
        {
            if (string.IsNullOrEmpty(varRootPath))
            {
                return;
            }

            // 以打开的目录作为工作区，建立 .psp 隐藏缓存目录
            if (Directory.Exists(varRootPath))
            {
                CurrentWorkspace = new Workspace(varRootPath);
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
                // 跳过工作区缓存目录（.psp）以及其它隐藏目录
                if (string.Equals(dirs[i].Name, Workspace.CacheDirName, StringComparison.OrdinalIgnoreCase))
                    continue;

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
