using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PastMediaPlayer_Project
{
    /// <summary>
    /// 单个标签的索引条目：记录所有打了该标签的视频路径，以及总数
    /// </summary>
    public class TagIndexEntry
    {
        /// <summary>标签名</summary>
        public string Name { get; set; }

        /// <summary>打了该标签的视频相对路径集合（与 WorkspaceCache.Entries 的 Key 一致）</summary>
        public List<string> Paths { get; set; } = new List<string>();

        /// <summary>视频数量（= Paths.Count，写入便于直接读取）</summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 工作区标签索引：标签 -> 视频路径列表 的倒排索引。
    /// 由 <see cref="WorkspaceCache"/> 根据 Entries 自动重建并落盘为 .psp/tag_index.json
    /// </summary>
    public class TagIndex
    {
        public const string IndexFileName = "tag_index.json";

        public int Version { get; set; } = 1;

        /// <summary>Key = 标签名（不区分大小写），Value = 索引条目</summary>
        public Dictionary<string, TagIndexEntry> Tags { get; set; }
            = new Dictionary<string, TagIndexEntry>(StringComparer.OrdinalIgnoreCase);

        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true,
        };

        /// <summary>从工作区读取索引（若不存在则返回空索引）</summary>
        public static TagIndex Load(Workspace workspace)
        {
            if (workspace == null) throw new ArgumentNullException(nameof(workspace));

            TagIndex index = null;
            if (workspace.CacheDirExists)
            {
                string text = workspace.ReadCacheText(IndexFileName);
                if (!string.IsNullOrEmpty(text))
                {
                    try { index = JsonSerializer.Deserialize<TagIndex>(text, s_jsonOptions); }
                    catch { index = null; }
                }
            }

            if (index == null) index = new TagIndex();
            // 反序列化后字典默认大小写敏感，重建以保持不敏感
            index.Tags = new Dictionary<string, TagIndexEntry>(
                index.Tags ?? new Dictionary<string, TagIndexEntry>(),
                StringComparer.OrdinalIgnoreCase);
            return index;
        }

        /// <summary>把索引写入工作区缓存目录</summary>
        public void Save(Workspace workspace)
        {
            if (workspace == null) throw new ArgumentNullException(nameof(workspace));
            string json = JsonSerializer.Serialize(this, s_jsonOptions);
            workspace.WriteCacheText(IndexFileName, json);
        }

        /// <summary>根据 entries 全量重建索引（清空后重新插入）</summary>
        public void Rebuild(IDictionary<string, MediaCacheEntry> entries)
        {
            Tags.Clear();
            if (entries == null) return;

            foreach (var kv in entries)
            {
                string path = kv.Key;
                var entry = kv.Value;
                if (entry == null || entry.Tags == null) continue;

                for (int i = 0; i < entry.Tags.Count; i++)
                {
                    string tag = (entry.Tags[i] ?? string.Empty).Trim();
                    if (tag.Length == 0) continue;

                    if (!Tags.TryGetValue(tag, out var ti))
                    {
                        ti = new TagIndexEntry { Name = tag };
                        Tags[tag] = ti;
                    }
                    // 同一路径不重复加入
                    if (ti.Paths.IndexOf(path) < 0)
                    {
                        ti.Paths.Add(path);
                    }
                }
            }

            // 同步 Count
            foreach (var ti in Tags.Values) ti.Count = ti.Paths.Count;
        }

        /// <summary>查询某个标签下所有视频路径</summary>
        public IReadOnlyList<string> GetPathsByTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return Array.Empty<string>();
            return Tags.TryGetValue(tag, out var ti) ? ti.Paths : (IReadOnlyList<string>)Array.Empty<string>();
        }

        /// <summary>查询某个标签下视频数量</summary>
        public int GetCount(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return 0;
            return Tags.TryGetValue(tag, out var ti) ? ti.Count : 0;
        }
    }
}
