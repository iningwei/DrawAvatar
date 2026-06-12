using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UGUIRotateZ : MonoBehaviour
{
    public bool isclockwise = true;
    public float speed = 60f;//每秒多少度
    public float z = 0;
    RectTransform target;

    void Start()
    {
        target = GetComponent<RectTransform>();
        target.localEulerAngles = new Vector3(0, 0, z);
    }


    void Update()
    {
        if (isclockwise)
        {
            z -= Time.deltaTime * speed;
        }
        else
        {
            z += Time.deltaTime * speed;
        }

        target.localEulerAngles = new Vector3(0, 0, z);
    }
}
