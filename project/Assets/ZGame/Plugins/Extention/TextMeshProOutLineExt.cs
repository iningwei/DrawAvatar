using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshProOutLineExt : MonoBehaviour
{
    public float OutLineWidth;
    public Color OutLineColor;

    TMPro.TextMeshProUGUI tmp;
    void Start()
    {
        //最终好像也是修改的材质球shader？运行过程中会生成新的材质球实例
        //fontMaterial和fontSharedMaterial 或许一个自己的，一个共享的？
        tmp = GetComponent<TMPro.TextMeshProUGUI>();
        tmp.outlineWidth = OutLineWidth;
        tmp.outlineColor = OutLineColor;
        tmp.fontMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        tmp = GetComponent<TMPro.TextMeshProUGUI>();
        tmp.outlineWidth = OutLineWidth;
        tmp.outlineColor = OutLineColor;
#endif
    }
}
