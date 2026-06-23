using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PastMediaPlayer_Project
{
    /// <summary>
    /// 单个媒体文件的缓存条目。
    /// 通过新增字段或往 Extra 里塞自定义键值对来扩展。
    /// </summary>
    public class MediaCacheEntry
    {
        /// <summary>别名</summary>
        public string Alias { get; set; }

        /// <summary>评分（0.1-10，支持一位小数），null 表示未评分</summary>
        public double? Rating { get; set; }

        /// <summary>标签列表</summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>最近一次播放进度（秒），便于后续断点续播</summary>
        public double LastPositionSec { get; set; }

        /// <summary>最近一次播放时间</summary>
        public DateTime? LastPlayedAt { get; set; }

        /// <summary>
        /// 通用扩展字段，未来新增信息可直接写入这里，避免频繁改 schema。
        /// 使用 JsonElement 以保留任意 JSON 结构。
        /// </summary>
        [JsonPropertyName("extra")]
        public Dictionary<string, JsonElement> Extra { get; set; } = new Dictionary<string, JsonElement>();
    }

    /// <summary>
    /// 工作区级别的媒体缓存集合：
    /// Key  = 视频相对工作区根目录的相对路径（统一使用 '/'，大小写不敏感）
    /// Value = <see cref="MediaCacheEntry"/>
    /// 持久化为 .psp/media_cache.json
    /// </summary>
    public class WorkspaceCache
    {
        public const string CacheFileName = "media_cache.json";

        /// <summary>schema 版本号，便于后续升级迁移</summary>
        public int Version { get; set; } = 1;

        public Dictionary<string, MediaCacheEntry> Entries { get; set; }
            = new Dictionary<string, MediaCacheEntry>(StringComparer.OrdinalIgnoreCase);

        private Workspace _workspace;
        private TagIndex _tagIndex;

        /// <summary>标签倒排索引（懒加载）：Tag -> 所有打了该标签的视频路径列表。
        /// 在 <see cref="Save"/> 时会以 Entries 为唯一数据源重建后落盘。</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public TagIndex TagIndex
        {
            get
            {
                if (_tagIndex == null && _workspace != null) _tagIndex = TagIndex.Load(_workspace);
                return _tagIndex;
            }
        }

        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
        };

        /// <summary>构造一个绑定到指定工作区的缓存（不会自动加载）</summary>
        public static WorkspaceCache Load(Workspace workspace)
        {
            if (workspace == null) throw new ArgumentNullException(nameof(workspace));

            WorkspaceCache cache = null;
            if (workspace.CacheDirExists)
            {
                string text = workspace.ReadCacheText(CacheFileName);
                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {
                        cache = JsonSerializer.Deserialize<WorkspaceCache>(text, s_jsonOptions);
                    }
                    catch
                    {
                        // 文件损坏时丢弃旧数据，避免崩溃
                        cache = null;
                    }
                }
            }

            if (cache == null) cache = new WorkspaceCache();
            // 反序列化后字典默认是大小写敏感的，这里重建一次以保证 OrdinalIgnoreCase
            cache.Entries = new Dictionary<string, MediaCacheEntry>(
                cache.Entries ?? new Dictionary<string, MediaCacheEntry>(),
                StringComparer.OrdinalIgnoreCase);
            cache._workspace = workspace;
            return cache;
        }

        /// <summary>保存到 .psp/media_cache.json，首次写入会触发 .psp 目录创建</summary>
        public void Save()
        {
            if (_workspace == null) throw new InvalidOperationException("WorkspaceCache 未绑定 Workspace");
            string json = JsonSerializer.Serialize(this, s_jsonOptions);
            _workspace.WriteCacheText(CacheFileName, json);

            // 同步重建并落盘标签索引，保证与 Entries 一致
            if (_tagIndex == null) _tagIndex = TagIndex.Load(_workspace);
            _tagIndex.Rebuild(Entries);
            _tagIndex.Save(_workspace);
        }

        /// <summary>把绝对路径转换为相对工作区根目录的标准化 Key</summary>
        public string MakeKey(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) || _workspace == null) return null;
            string root = Path.GetFullPath(_workspace.RootPath);
            string full = Path.GetFullPath(fullPath);

            if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return null;

            string rel = full.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return rel.Replace('\\', '/');
        }

        /// <summary>按相对路径获取条目，不存在返回 null</summary>
        public MediaCacheEntry Get(string relativeKey)
        {
            if (string.IsNullOrEmpty(relativeKey)) return null;
            return Entries.TryGetValue(relativeKey, out var e) ? e : null;
        }

        /// <summary>按相对路径获取或创建条目</summary>
        public MediaCacheEntry GetOrCreate(string relativeKey)
        {
            if (string.IsNullOrEmpty(relativeKey))
                throw new ArgumentException("relativeKey 不能为空", nameof(relativeKey));
            if (!Entries.TryGetValue(relativeKey, out var entry))
            {
                entry = new MediaCacheEntry();
                Entries[relativeKey] = entry;
            }
            return entry;
        }

        /// <summary>按绝对路径获取或创建条目</summary>
        public MediaCacheEntry GetOrCreateByFullPath(string fullPath)
        {
            string key = MakeKey(fullPath);
            return key == null ? null : GetOrCreate(key);
        }

        public bool Remove(string relativeKey)
        {
            return !string.IsNullOrEmpty(relativeKey) && Entries.Remove(relativeKey);
        }
    }
}
