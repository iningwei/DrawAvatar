 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MemoryPoolItem : MonoBehaviour
{
    private MemoryPool<GameObject> gameObjectPool;
    public MemoryPoolItem SetPool(MemoryPool<GameObject> pool) { gameObjectPool = pool; return this; }
    static private bool shuttingDown = false;
    static public void SetShutdown(bool sceneIsEnding) { shuttingDown = sceneIsEnding; }
#if FAIL_FAST
    void OnApplicationQuit() { SetShutdown(true); }
    void Start() { SetShutdown(false); }
    void OnDestroy() {
        if(!shuttingDown) throw new System.Exception("Instead of Object.Destroy("+gameObject+"), call MemoryPoolItem.Destroy("+gameObject+")\n"
            +"When changing levels, call MemoryPoolItem.SetShutdown(true) first");
    }
#endif
    public void FreeSelf() { gameObjectPool.Free(gameObject); }
    /// <summary>If the given GameObject belongs to a memory pool, mark it as free in that pool. Otherwise, Object.Destroy()</summary>
    static public void Destroy(GameObject go)
    {
        MemoryPoolItem i = go.GetComponent<MemoryPoolItem>();
        if (i != null) { i.FreeSelf(); }
        else { Object.Destroy(go); }
    }
}
