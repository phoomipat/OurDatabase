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
        private GenericNode<TKey, TData> First => storageService.First;
        private readonly Func<TKey, TKey, int> comparer;
        private readonly StorageService<TKey, TData> storageService;
        private int LatestIndex => storageService.Count - 1;

        public GenericTree(string indexName, Func<TKey, TKey, int> comparer)
        {
            this.comparer = comparer;
            this.storageService = new StorageService<TKey, TData>(indexName);
        }

        public void AddData(TKey key, TData data)
        {
            storageService.Count++;
            if (storageService.First == null)
            {
                storageService.First = new GenericNode<TKey, TData>(key, data, LatestIndex);
                storageService.WriteAtIndex(storageService.First, LatestIndex);
                return;
            }
            AddData(storageService.First, key, data);
        }

        private int AddData(GenericNode<TKey, TData> cur, TKey key, TData data)
        {
            if (cur == null)
            {
                storageService.WriteAtIndex(new GenericNode<TKey, TData>(key, data, LatestIndex), LatestIndex);
                return LatestIndex;
            }
                
            if (comparer(key, cur.Key) <= 0)
            { 
                var index = AddData( storageService.ReadAt(cur.LeftPosition), key, data);
                if (index == LatestIndex)
                {
                    cur.LeftPosition = index;
                    storageService.WriteAtIndex(cur, cur.NodePosition);
                }
            }
            else
            {
                var index = AddData(storageService.ReadAt(cur.RightPosition), key, data);
                if (index == LatestIndex)
                {
                    cur.RightPosition = index;
                    storageService.WriteAtIndex(cur, cur.NodePosition);
                }
            }
            return cur.NodePosition;
        }

        public GenericNode<TKey, TData> Search(TKey searchKey)
        {
            return Search(First, searchKey);
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