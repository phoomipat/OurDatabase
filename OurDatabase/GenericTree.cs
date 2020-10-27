using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OurDatabase
{
    public class GenericTree<TKey, TData>
    {
        private GenericNode<TKey, TData> First => DataCount != 0 ? storageService.ReadAt(0) : null;
        private int DataCount => storageService.Count;
        private readonly Func<TKey, TKey, int> comparer;
        private readonly StorageService<TKey, TData> storageService;
        

        public GenericTree(string indexName, Func<TKey, TKey, int> comparer)
        {
            this.comparer = comparer;
            this.storageService = new StorageService<TKey, TData>(indexName);
        }

        public void AddData(TKey key, TData data) => AddData(First, key, data);
        public GenericNode<TKey, TData> Search(TKey searchKey) => Search(First, searchKey);
        

        private int AddData(GenericNode<TKey, TData> cur, TKey key, TData data)
        {
            if (cur == null)
            {
                var writtenIndex = DataCount;
                storageService.WriteAtIndex(new GenericNode<TKey, TData>(key, data, DataCount), DataCount);
                return writtenIndex;
            }
                
            if (comparer(key, cur.Key) <= 0)
            { 
                var index = AddData( storageService.ReadAt(cur.LeftPosition), key, data);
                if (index == DataCount - 1)
                {
                    cur.LeftPosition = index;
                    storageService.WriteAtIndex(cur, cur.NodePosition);
                }
            }
            else
            {
                var index = AddData(storageService.ReadAt(cur.RightPosition), key, data);
                if (index == DataCount - 1)
                {
                    cur.RightPosition = index;
                    storageService.WriteAtIndex(cur, cur.NodePosition);
                }
            }
            return cur.NodePosition;
        }
        
        private GenericNode<TKey, TData> Search(GenericNode<TKey, TData> cur, TKey searchKey)
        {
            if (cur == null)
                return null;
            if (comparer(cur.Key, searchKey) == 0)
                return cur;
            if (comparer(searchKey, cur.Key) <= 0)
                return Search(storageService.ReadAt(cur.LeftPosition), searchKey);
            return Search(storageService.ReadAt(cur.RightPosition), searchKey);
        }

        public override string ToString()
        {
            return string
                .Join("  ,  ", storageService.ReadAll()
                .Select(n => $"Index: {n.NodePosition} {n.LeftPosition}{n.Value.ToString()}{n.RightPosition}"));
        }

    }
}