using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    static T _instance;
    static object syncRoot = new System.Object();
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }

            return _instance;
        }
    }

    public virtual void Update()
    {

    }
    public virtual void LateUpdate()
    {

    }

    public virtual void FixedUpdate()
    {

    }
    public virtual void OnDestroy()
    {

    }
}
