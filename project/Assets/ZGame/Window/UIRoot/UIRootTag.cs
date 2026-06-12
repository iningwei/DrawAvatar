using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class UIRootTag : MonoBehaviour
{
    //Node类型不能作为directChildUIRoots，只能由HolderTag关联
    public List<UIRootTag> directChildUIRoots;
}
