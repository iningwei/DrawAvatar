using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using ZGame;
using ZGame.Ress;
using ZGame.Window;

public class SceneUtils
{
    public static void OpenSceneWithLoading(string sceneName, LoadSceneMode loadMode, Action sceneLoadCallback, Action loadingFinishedCallback, Action loadingOnShownCallback)
    {
        AudioManager.Instance.StopBGM();

        Action loadingFinished = () =>
        {
            SetUICameraOverlay();
            loadingFinishedCallback?.Invoke();

            ////TODO：播放背景音乐
            //var data = BeanManager.Instance.GetMapData(ServiceFetch.mapService.curMapId);
            //if (data != null)
            //{
            //    GameUtils.PlayBGM(data.BGMId);
            //}
        };

        WindowManager.Instance.ShowWindow(WindowNames.LoadingWindow, WindowLayer.SceneChange, false, false, true, (windowObj) =>
        {
            loadingOnShownCallback?.Invoke();
            SetUICameraBase();
            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
            {
                var asyncOp = SceneManager.LoadSceneAsync(sceneName, loadMode);
                asyncOp.completed += (op) =>
                {
                    WindowManager.Instance.SendWindowMessage(WindowNames.LoadingWindow, WindowMsgID.OnSceneLoadSuccess);
                    sceneLoadCallback?.Invoke();

                };
            }
            else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
            {
                AAManager.Instance.LoadScene(sceneName, loadMode, () =>
                {
                    WindowManager.Instance.SendWindowMessage(WindowNames.LoadingWindow, WindowMsgID.OnSceneLoadSuccess);
                    sceneLoadCallback?.Invoke();
                });
            }
        }, loadingFinished);
    }

    public static void LoadScene(string sceneName, Action loadingOnShownCallback, Action loadingFinishedCallback)
    {
        SceneUtils.OpenSceneWithLoading(sceneName, LoadSceneMode.Single, loadingFinishedCallback, null, loadingOnShownCallback);
    }





    public static void SetUICameraOverlay()
    {
        Debug.LogError("call SetUICameraOverlay");
        var uiCam = WindowManager.Instance.GetRootUICamera();
        if (uiCam != null)
        {
            if (Camera.main == null)
            {
                Debug.LogError("camera main is null");
            }
            else
            {
                GameUtils.SetUICamOverlayAndBelongBaseCam(uiCam, Camera.main);

                uiCam.depth = -2f;
            }
        }
        else
        {
            Debug.LogError("no uiCam");
        }
    }

    public static void SetUICameraBase()
    {
        Debug.LogError("call SetUICameraBase");
        if (Camera.main != null)
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Clear();
        }

        var uiCam = WindowManager.Instance.GetRootUICamera();
        if (uiCam != null)
        {
            var uiCamData = uiCam.GetUniversalAdditionalCameraData();
            uiCamData.renderType = CameraRenderType.Base;

            uiCam.depth = 0f;
        }
        else
        {
            Debug.LogError("no uiCam");
        }
    }
}
