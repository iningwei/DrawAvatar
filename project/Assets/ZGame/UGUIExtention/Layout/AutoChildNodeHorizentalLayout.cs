using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoChildNodeHorizentalLayout : MonoBehaviour
{
    public float childWidth;
    float totalWidth;
    float leftXPos;

    void Start()
    {
        this.autoLayout();
    }

    float delay = 0.1f;


    private void Update()
    {
        this.delay -= Time.deltaTime;
        if (this.delay < 0)
        {
            this.autoLayout();
            this.delay = 0.1f;
        }
    }

    void autoLayout()
    {
        totalWidth = this.GetComponent<RectTransform>().rect.width;
        leftXPos = -totalWidth * 0.5f;
        var childCount = this.transform.childCount;
        float gap = this.totalWidth / (childCount - 1);

        Transform child;
        for (int i = 0; i < childCount; i++)
        {
            child = this.transform.GetChild(i);
            child.localPosition = new Vector3(leftXPos, 0f, 0f);
            leftXPos = leftXPos + gap;
        }
    }
}
