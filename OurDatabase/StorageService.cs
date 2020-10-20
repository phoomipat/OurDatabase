using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OurDatabase
{
    public class StorageService<TKey, TData>
    {
        private int objectByteLength;
        private readonly Stream stream;
        public GenericNode<TKey, TData> First;
        public int Count;
        
        public StorageService(string indexName)
        {
            stream = File.Open($"{indexName}", FileMode.OpenOrCreate);
            SetObjectByteLength();
            InitData();
        }
        
        public void InitData()
        {
            var result = ReadAll();
            First = ReadAll().FirstOrDefault();
            Count = result.Length;
        }

        private void SetObjectByteLength()
        {
            foreach (var f in typeof(GenericNode<TKey, TData>).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var fieldType = f.FieldType;

                if (fieldType == typeof(int))
                {
                    objectByteLength += 4;
                }
                else if (fieldType == typeof(string))
                {
                    objectByteLength += 256;
                }
                else
                {
                    foreach (var f1 in typeof(TData).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        var fieldType1 = f1.FieldType;

                        if (fieldType1 == typeof(int))
                            objectByteLength += 4;
                        else if (fieldType1 == typeof(string))
                            objectByteLength += 256;
                        else
                            throw new ArgumentException($"Database doesn't support data type {fieldType1}");
                    }
                }
            }
        }
        
        public GenericNode<TKey, TData>[] ReadAll()
        {
            var objList = new List<GenericNode<TKey, TData>>();
            var buffer = new byte[objectByteLength];
            
            stream.Seek(0, SeekOrigin.Begin);
            while ((stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                objList.Add(ReadNodeObject(buffer));
            }

            return objList.ToArray();
        }

        public GenericNode<TKey, TData> ReadAt(int index)
        {
            if (index == -1)
                return null;
            
            var buffer = new byte[objectByteLength];
            stream.Seek(index*objectByteLength, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            return ReadNodeObject(buffer);
        }

        private static GenericNode<TKey, TData> ReadNodeObject(byte[] buffer)
        {
            var offset = 0;
            return (GenericNode<TKey, TData>) Activator.CreateInstance(typeof(GenericNode<TKey, TData>), typeof(GenericNode<TKey, TData>)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Select(f => f.FieldType)
                .Select(fieldType => ReadNodeData(fieldType, buffer, ref offset)).ToArray());
        }

        private static object ReadNodeData(Type fieldType, byte[] buffer, ref int offset)
        {
            if (fieldType == typeof(int))
            {
                var bytes = BitConverter.ToInt32(buffer, offset);
                offset += 4;
                return bytes;
            }

            if (fieldType == typeof(string))
            {
                var bytes = Encoding.UTF8.GetString(buffer, offset, 256);
                offset += 256;
                return bytes;
            }
            else
            {
                var TDataParams = new List<object>();
                foreach (var f1 in typeof(TData).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    var fieldType1 = f1.FieldType;

                    if (fieldType1 == typeof(int))
                    {
                        TDataParams.Add(BitConverter.ToInt32(buffer, offset));
                        offset += 4;
                    }
                    else if (fieldType1 == typeof(string))
                    {
                        TDataParams.Add(Encoding.UTF8.GetString(buffer, offset, 256));
                        offset += 256;
                    }
                    else
                        throw new ArgumentException($"Database doesn't support data type {fieldType1}");
                }

                return (TData) Activator.CreateInstance(typeof(TData), TDataParams.ToArray());
            }
        }

        public void WriteAtIndex(GenericNode<TKey, TData> input, int index)
        {
            Console.WriteLine($"Writing {input.Key}");
            var byteToWrite = new List<byte>();
            foreach (var f in typeof(GenericNode<TKey, TData>).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var fieldType = f.FieldType;
                var fieldVal = f.GetValue(input);

                if (fieldType == typeof(int))
                    byteToWrite.AddRange(ToBytes((int) fieldVal));
                else if (fieldType == typeof(string))
                    byteToWrite.AddRange(ToBytes((string) fieldVal));
                else if (fieldType == typeof(TKey))
                    byteToWrite.AddRange(ToBytes<TKey>(fieldVal));
                else if (fieldType == typeof(TData))
                    byteToWrite.AddRange(ToBytes<TData>(fieldVal));
                else
                    throw new Exception($"Not supported {fieldType}");
            }

            stream.Seek(index*objectByteLength, SeekOrigin.Begin);
            Write(byteToWrite.ToArray());
        }
        
        private void Write(byte[] input)
        {
            stream.Write(input);
            stream.Flush();
        }
        
        private static IEnumerable<byte> ToBytes<T>(object input)
        {
            var byteToWrite = new List<byte>();
            foreach (var f in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var fieldType = f.FieldType;
                var fieldVal = f.GetValue(input);

                if (fieldType == typeof(int))
                    byteToWrite.AddRange(ToBytes((int) fieldVal));
                else if (fieldType == typeof(string))
                    byteToWrite.AddRange(ToBytes((string) fieldVal));
                else
                    throw new Exception("Not supported");
            }
            return byteToWrite.ToArray();
        }

        private static IEnumerable<byte> ToBytes(string input)
        {
            var dest = new byte[256];
            var stringBytes = Encoding.UTF8.GetBytes(input);
            Array.Copy(stringBytes, 0, dest, 0, stringBytes.Length);
            return dest;
        }

        private static IEnumerable<byte> ToBytes(int input) => BitConverter.GetBytes(input);
        
        public override string ToString()
        {
            return string
                .Join("  ,  ", ReadAll()//.OrderBy(n => n.Key)
                .Select(n => $"Index: {n.NodePosition} {n.LeftPosition}{n.Value.ToString()}{n.RightPosition}"));
        }

    }
}