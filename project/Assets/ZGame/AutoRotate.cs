using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float speed = 50f;

    public bool flag = false;
    public bool autoRotate = true;
    public void StopRotate()
    {
        this.flag = false;
    }
    public void StartRotate()
    {
        this.flag = true;
    }
    void Update()
    {
        if (flag && autoRotate)
        {
            this.transform.RotateAround(this.transform.position, Vector3.up, speed * Time.deltaTime);
        }
    }
}
