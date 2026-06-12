using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://www.cnblogs.com/murongxiaopifu/p/7050233.html

[ExecuteInEditMode]
public class RenderDepth : MonoBehaviour
{
    //相机渲染模式要手动选择延迟渲染
    //下述设置没有用。。。
    void OnEnable()
    {
        //GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
