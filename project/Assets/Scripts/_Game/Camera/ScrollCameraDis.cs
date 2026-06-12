using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

public class ScrollCameraDis : MonoBehaviour
{
    //////public float zoomSpeed = 40f;
    //////public float minDistance = 30f;
    //////public float maxDistance = 280f;

    //////GameCameraControl gameCameraControl;
    //////bool isInited = false;
    //////public void Init(GameCameraControl gcc)
    //////{
    //////    isInited = true;
    //////    this.gameCameraControl = gcc;
    //////}


    //////private void OnDestroy()
    //////{

    //////}
    //////private void Update()
    //////{
    //////    if (!isInited)
    //////    {
    //////        return;
    //////    }

    //////    float scroll = Input.GetAxis("Mouse ScrollWheel");

    //////    if (scroll != 0)
    //////    {
    //////        float newDistance = gameCameraControl.initDistance - scroll * zoomSpeed;
    //////        newDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
    //////        gameCameraControl.initDistance = newDistance;
    //////    }
    //////}
}
