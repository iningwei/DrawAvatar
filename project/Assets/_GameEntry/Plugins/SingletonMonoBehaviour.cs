using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static volatile T instance;
    private static object syncRoot = new Object();

    protected virtual bool persistAcrossScenes => true;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        T[] instances = FindObjectsOfType<T>();
                        if (instances != null)
                        {
                            for (var i = 0; i < instances.Length; i++)
                            {
                                Destroy(instances[i].gameObject);
                            }
                        }
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        instance = go.AddComponent<T>();

                        if (instance.GetComponent<SingletonMonoBehaviour<T>>().persistAcrossScenes)
                        {
                            DontDestroyOnLoad(go);
                        }
                    }
                }
            }
            return instance;
        }
    }

    public virtual void OnDestroy()
    {
        //https://discussions.unity.com/t/managedstaticreferences-prevents-objects-to-be-cleared-from-memory/86104/3
        if (instance == this)//must set instance null, otherwise may cause memory leak 
        {
            instance = null;
        }

        //由于我们设置了DontDestroyOnLoad，正常情况下在APP运行情况下是不会出现触发OnDestroy的。
    }

    public virtual void OnApplicationQuit()
    {

    }

}

