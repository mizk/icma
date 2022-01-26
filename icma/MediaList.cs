using libicma.utils;
using System.Collections.Generic;
namespace icma
{
    /// <summary>
    /// 文件列表
    /// </summary>
    public class MediaList
    {
        /// <summary>
        /// 分页每页数量
        /// </summary>
        public const int PageSize = 15;
        private static MediaList instance;
        private static readonly object mutex = new();
        private readonly Dictionary<string, Media> storage = new();
        private readonly List<string> files = new();
        public static MediaList Instance()
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        instance = new MediaList();
                    }
                }
            }
            return instance;
        }

        
        
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="files"></param>
        public void AddRange(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Add(file);
            }
        }
        public void Add(string file)
        {
            var uid = Utils.Hash(file.ToLower());
            if (storage.ContainsKey(uid))
            {
                return;
            }
            var item = new Media();
            item.Clone(file);
            if (item.IsValid)
            {
                storage[uid] = item;
                files.Add(uid);
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id)
        {
            if (files.Contains(id))
            {
                files.Remove(id);
            }
            if (storage.ContainsKey(id))
            {
                storage.Remove(id);
            }
        }
       

        public Media Get(string id)
        {
          if(storage.TryGetValue(id, out Media item))
            {
                return item;
            }
          return null;
        }
        
        /// <summary>
        /// 页数
        /// </summary>
        public int Page
        {
            get
            {
                var left = files.Count % PageSize;
                var page = (files.Count - left) / PageSize;
                return left > 0 ? page + 1 : page;
            }


        }
        private int currentPage = 0;
        /// <summary>
        /// 当前页索引
        /// </summary>
        public int CurrentPage => currentPage;
        /// <summary>
        /// 移动到指定页
        /// </summary>
        /// <param name="page"></param>
        public void MoveTo(int page)
        {
            if (page >= 0 && page < Page)
            {
                currentPage = page;
            }
        }

        /// <summary>
        /// 当前页数据集合
        /// </summary>
        /// <returns></returns>
        public ListRowModel CurrentItems()
        {
            return Items(currentPage);
        }
        /// <summary>
        /// 给定页索引处的数据集合
        /// </summary>
        /// <param name="page">索引</param>
        /// <returns></returns>
        private ListRowModel Items(int page)
        {
            var model = new ListRowModel();
            for (var index = 0; index < PageSize; index++)
            {
                var pos = page * PageSize + index;
                var id = files.At(pos);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                if (storage.ContainsKey(id))
                {
                    var item = storage[id];
                    model.Cells.Add(item);
                }
            }
            return model;
        }

    }
}
