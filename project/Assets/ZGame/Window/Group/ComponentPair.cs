using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ComponentPair
{
    [SerializeField]
    public GameObject target;

    public ValueType valueType;

    [SerializeField] private string stringValue;
    [SerializeField] private int intValue;
    [SerializeField] private float floatValue;
    [SerializeField] private bool boolValue;
    [SerializeField] private UnityEngine.Object objectValue;

    public object Value => valueType switch
    {
        ValueType.String => stringValue,
        ValueType.Int => intValue,
        ValueType.Float => floatValue,
        ValueType.Bool => boolValue,
        ValueType.Object => objectValue,
        _ => null
    };
}
