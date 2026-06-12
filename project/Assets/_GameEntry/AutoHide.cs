using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public float delay = 3f;

    float delayValue;
    private void OnEnable()
    {
        delayValue = delay;
    }
    void Update()
    {
        delayValue -= Time.deltaTime;
        if (delayValue < 0)
        {
            this.gameObject.SetActive(false);
        }
    }


}
