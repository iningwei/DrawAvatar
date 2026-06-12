using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class ComponentGroup<T> : MonoBehaviour where T : UnityEngine.Object
{
    [SerializeField]
    protected List<ComponentPair> pairs = new List<ComponentPair>();


    private List<(T key, object value)> _cachedKVs;
    private bool _isCacheDirty = true;

    public object this[T component] => GetValue(component);

    /// <summary>
    /// 获取所有有效的 (组件, Value) 对
    /// </summary>
    public IReadOnlyList<(T key, object value)> KvDatas
    {
        get
        {
            if (_isCacheDirty || _cachedKVs == null)
            {
                RefreshCache();
            }
            return _cachedKVs;
        }
    }

    /// <summary>
    /// 手动刷新缓存（当你在运行时动态修改 pairs 时需要调用）
    /// </summary>
    public void RefreshCache()
    {
        _cachedKVs = new List<(T, object)>();

        foreach (var p in pairs)
        {
            if (p.target == null)
                continue;

            T component = p.target.GetComponent<T>();
            if (component != null)
            {
                _cachedKVs.Add((component, p.Value));
            }
            else
            {
                Debug.LogWarning($"[ComponentGroup] 在 GameObject '{p.target.name}' 上找不到组件 {typeof(T).Name}", this);
            }
        }

        _isCacheDirty = false;
    }

    /// <summary>
    /// 根据组件获取对应的 Value
    /// </summary>
    public object GetValue(T component)
    {
        if (component == null) return null;

        foreach (var p in pairs)
        {
            if (p.target != null && p.target.GetComponent<T>() == component)
                return p.Value;
        }
        return null;
    }

    // 当Inspector中数据被修改时，标记缓存失效
    private void OnValidate()
    {
        _isCacheDirty = true;
    }
}