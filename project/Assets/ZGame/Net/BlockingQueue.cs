using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingQueue<T> where T : class
{
    object syncObj;
    Queue<T> mQueue;
    public BlockingQueue()
    {
        syncObj = new object();
        mQueue = new Queue<T>();
    }

    public void Enqueue(T t)
    {
        lock (syncObj)
        {
            mQueue.Enqueue(t);
        }
    }
    public T Dequeue()
    {
        lock (syncObj)
        {
            T ret = null;
            if (mQueue.Count > 0)
            {
                ret = mQueue.Dequeue();
            }
            return ret;
        }
    }

    public int Count()
    {
        lock (syncObj)
        {
            return mQueue.Count;
        }
    }

    public void Clear()
    {
        lock (syncObj)
        {
            mQueue.Clear();
        }
    }

}
