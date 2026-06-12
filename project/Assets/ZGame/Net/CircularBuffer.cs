using System;
using System.Collections;
using System.Collections.Generic;


namespace ZGame.Net
{
    public class CircularBuffer
    {
        [UnityEngine.SerializeField]
        private int size;
        [UnityEngine.SerializeField]
        private int head;
        private byte[] buffer;

        object lockObj = new object();

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("capacity must be greater than or equal to zero.",
                    "capacity");

            size = 0;
            head = 0;
            buffer = new byte[capacity];
        }

        public void Clear()
        {
            lock (lockObj)
            {
                size = 0;
                head = 0;
            }
        }

        public void Put(byte[] src, int offset, int count)
        {
            lock (lockObj)
            {
                if (count > buffer.Length - size)
                {
                    autoExpandCapacity(count + buffer.Length);
                }

                int srcIndex = offset;
                for (int i = 0; i < count; i++, srcIndex++)
                {
                    buffer[(i + head + size) % buffer.Length] = src[srcIndex];
                }
                size += count;
            }
        }

        void autoExpandCapacity(int newCapacity)
        {
            newCapacity = newCapacity > buffer.Length * 2 ? newCapacity : buffer.Length * 2;
            var newBuf = new byte[newCapacity];
            for (int j = 0, i = head; j < size; i++, j++)
            {
                newBuf[j] = buffer[i % buffer.Length];
            }
            head = 0;
            buffer = newBuf;
            //UnityEngine.Debug.LogError("自动扩容，扩容后大小为：" + newCapacity);
        }

        byte[] b4temp = new byte[4];
        byte[] b2temp = new byte[4];
        public Int32 ReadInt32()
        {
            lock (lockObj)
            {
                var data = ReadBytes(4, false);
                int ret = BitConverter.ToInt32(data, 0);
                return ret;
            }
        }

        public short ReadInt16()
        {
            lock (lockObj)
            {
                var data = ReadBytes(2, false);
                short ret = BitConverter.ToInt16(data, 0);
                return ret;
            }
        }

        public byte ReadByte(bool islock = true)
        {
            byte ret = 0;
            if (islock)
            {
                lock (lockObj)
                {
                    if (size < 1)
                    {
                        UnityEngine.Debug.LogError("ReadBytes buff size error:" + size.ToString());
                        return ret;
                    }

                    int h = head;
                    ret = buffer[h++];
                    h %= buffer.Length;

                    size -= 1;
                    head = h;
                    return ret;
                }
            }
            else
            {
                if (size < 1)
                {
                    UnityEngine.Debug.LogError("ReadBytes buff size error:" + size.ToString());
                    return ret;
                }

                int h = head;
                ret = buffer[h++];
                h %= buffer.Length;

                size -= 1;
                head = h;
                return ret;
            }
        }

        public byte[] ReadBytes(int count, bool islock = true)
        {
            if (islock)
            {
                lock (lockObj)
                {
                    if (size < count)
                    {
                        UnityEngine.Debug.LogError("ReadBytes buff size error:" + size.ToString());
                        return null;
                    }

                    if (count < 0)
                    {
                        UnityEngine.Debug.LogError("ReadBytes count error:" + count.ToString());
                    }

                    byte[] btArr = new byte[count];
                    int h = head;
                    for (int i = 0; i < count; i++)
                    {
                        btArr[i] = buffer[h++];
                        h %= buffer.Length;
                    }
                    size -= count;
                    head = h;

                    return btArr;
                }
            }
            else
            {
                if (size < count)
                {
                    UnityEngine.Debug.LogError("ReadBytes buff size error:" + size.ToString());
                    return null;
                }

                if (count < 0)
                {
                    UnityEngine.Debug.LogError("ReadBytes count error:" + count.ToString());
                }

                byte[] btArr = new byte[count];
                int h = head;
                for (int i = 0; i < count; i++)
                {
                    btArr[i] = buffer[h++];
                    h %= buffer.Length;
                }
                size -= count;
                head = h;

                return btArr;
            }

        }


        public void CopyTo(byte[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, size);
        }

        public void CopyTo(int index, byte[] array, int arrayIndex, int count)
        {
            lock (lockObj)
            {
                if (count > size)
                {
                    UnityEngine.Debug.LogError("CopyTo (count > size)");
                    return;
                }

                int bufferIndex = head;
                for (int i = 0; i < count; i++, bufferIndex++, arrayIndex++)
                {
                    array[arrayIndex] = buffer[bufferIndex];
                }
            }
        }

        public int Count
        {
            get
            {
                lock (lockObj)
                {
                    return size;
                }
            }
        }
    }

}