using System;
using System.IO;

namespace PastMediaPlayer_Project
{
    /// <summary>
    /// 工作区：以打开的文件夹为根，统一管理该目录下的缓存等元数据。
    /// 缓存文件统一存放在工作区根目录下的 .psp 隐藏文件夹中。
    /// </summary>
    public class Workspace
    {
        public const string CacheDirName = ".psp";

        public string RootPath { get; private set; }

        /// <summary>
        /// 工作区缓存目录（绝对路径），位于 RootPath/.psp
        /// </summary>
        public string CacheDir { get; private set; }

        public Workspace(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException("rootPath 不能为空", nameof(rootPath));
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"工作区根目录不存在: {rootPath}");

            RootPath = rootPath;
            CacheDir = Path.Combine(RootPath, CacheDirName);
            // 注意：构造时不创建 .psp 目录，延迟到首次写入缓存时再创建
        }

        /// <summary>
        /// 缓存目录是否已经存在（即是否已经产生过缓存文件）
        /// </summary>
        public bool CacheDirExists
        {
            get { return Directory.Exists(CacheDir); }
        }

        private WorkspaceCache _cache;

        /// <summary>
        /// 工作区媒体缓存（懒加载）。首次访问时从 .psp/media_cache.json 读取，
        /// 修改后需要调用 Save() 才会落盘，落盘时才会创建 .psp 目录。
        /// </summary>
        public WorkspaceCache Cache
        {
            get
            {
                if (_cache == null) _cache = WorkspaceCache.Load(this);
                return _cache;
            }
        }

        /// <summary>
        /// 确保缓存目录存在，并设置为隐藏属性。仅在首次写入缓存时才会真正创建。
        /// </summary>
        private void EnsureCacheDir()
        {
            if (!Directory.Exists(CacheDir))
            {
                DirectoryInfo info = Directory.CreateDirectory(CacheDir);
                // 新建时立即设置为隐藏
                info.Attributes |= FileAttributes.Hidden;
            }
        }

        /// <summary>
        /// 获取工作区缓存目录下某个相对路径的完整路径
        /// </summary>
        public string GetCachePath(string relativeName)
        {
            if (string.IsNullOrEmpty(relativeName))
                return CacheDir;
            return Path.Combine(CacheDir, relativeName);
        }

        /// <summary>
        /// 判断给定的绝对路径是否位于工作区缓存目录中
        /// </summary>
        public bool IsInCacheDir(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return false;
            string normalized = Path.GetFullPath(fullPath);
            string cacheNormalized = Path.GetFullPath(CacheDir);
            return normalized.StartsWith(cacheNormalized, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取工作区缓存目录下的文本文件。若 .psp 目录尚未创建则直接返回空。
        /// </summary>
        public string ReadCacheText(string relativeName)
        {
            if (!CacheDirExists) return string.Empty;
            string path = GetCachePath(relativeName);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        /// <summary>
        /// 写入工作区缓存目录下的文本文件。首次写入时才创建 .psp 隐藏目录。
        /// </summary>
        public void WriteCacheText(string relativeName, string content)
        {
            EnsureCacheDir();
            string path = GetCachePath(relativeName);
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, content ?? string.Empty);
        }
    }
}
