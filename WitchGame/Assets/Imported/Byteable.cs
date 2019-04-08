using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byteable
{
    public interface IByteable
    {
        void Write(ByteScribe writer);
        void Read(ByteScribe reader);
        int GetSize();
    }
    public partial class ByteScribe
    {
        private readonly byte[] stream;
        private int count;
        public int Count { get { return count; } }
        public ByteScribe(byte[] stream)
        {
            count = 0;
            this.stream = stream;
        }
        public void Reset()
        {
            count = 0;
        }

        public void Write(byte value)
        {
            stream[count] = value;
            count++;
        }
        public void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int i = 0;
            while (i < bytes.Length)
            {
                stream[count] = bytes[i];
                count++;
                i++;
            }
        }
        public void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int i = 0;
            while (i < bytes.Length)
            {
                stream[count] = bytes[i];
                count++;
                i++;
            }
        }
        public void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int i = 0;
            while (i < bytes.Length)
            {
                stream[count] = bytes[i];
                count++;
                i++;
            }
        }
        public void Write(string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            int i = 0;
            Write(bytes.Length);
            while (i < bytes.Length)
            {
                stream[count] = bytes[i];
                count++;
                i++;
            }
        }
        public void Write(IByteable value)
        {
            value.Write(this);
        }

        public byte ReadByte()
        {
            byte value = stream[count];
            count++;
            return value;
        }
        public int ReadInt()
        {
            int value = BitConverter.ToInt32(stream, count);
            count += sizeof(int);
            return value;
        }
        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(stream, count);
            count += sizeof(float);
            return value;
        }
        public string ReadString()
        {
            int length = ReadInt();
            string value = Encoding.ASCII.GetString(stream, count, length);
            count += length;
            return value;
        }
        public T Read<T>() where T : IByteable, new()
        {
            T value = new T();
            value.Read(this);
            return value;
        }
    }

    //Example of intferace implementation
    public struct Point : IByteable
    {
        public int x, y;
        public Point(int X, int Y)
        {
            x = X;
            y = Y;
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        //members must be read and written in the same order
        void IByteable.Write(ByteScribe writer)
        {
            writer.Write(x);
            writer.Write(y);
        }

        void IByteable.Read(ByteScribe writer)
        {
            x = writer.ReadInt();
            y = writer.ReadInt();
        }

        int IByteable.GetSize()
        {
            return sizeof(int) * 2;
        }
    }
    
    public static class IO
    {
        public static void WriteToFile(IByteable byteable, string fileName)
        {
            byte[] b = new byte[byteable.GetSize()];
            ByteScribe writer = new ByteScribe(b);
            writer.Write(byteable);
            File.WriteAllBytes(fileName, b);
        }
        public static T ReadFromFile<T>(string fileName) where T : IByteable, new()
        {
            byte[] b = File.ReadAllBytes(fileName);
            ByteScribe reader = new ByteScribe(b);
            return reader.Read<T>();
        }
    }

}
