using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class GRender : MonoBehaviour
{
    [SerializeField]
    private Camera blackCam;
    [SerializeField]
    private Camera whiteCam;
    //////[SerializeField]
    //////private float m_OrthographicSize = 3.2f;
    [SerializeField]
    private Vector2 rtSize;
    [SerializeField]
    private RenderTexture blackRT;
    [SerializeField]
    private RenderTexture whiteRT;
    [SerializeField]
    private Material rawImgMat;
    [SerializeField]
    private RawImage rawImg;

    public Transform refCamTran;

    bool enableUpdateCamPR2RefCam = false;
    public void SetUpdatePR2RefCam(bool status)
    {
        enableUpdateCamPR2RefCam = status;
    }
    private void LateUpdate()
    {
        if (refCamTran != null && enableUpdateCamPR2RefCam)
        {
            if (blackCam != null)
            {
                blackCam.transform.position = refCamTran.position;
                blackCam.transform.rotation = refCamTran.rotation;
            }
            if (whiteCam != null)
            {
                whiteCam.transform.position = refCamTran.position;
                whiteCam.transform.rotation = refCamTran.rotation;
            }
        }
    }


    public void StartRender(Vector2 rtSize, RawImage rawImg, Transform refCamTran, float shaderAdditiveOffset = 255, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGBHalf)
    {
        this.rtSize = rtSize;
        this.rawImg = rawImg;
        if (refCamTran.gameObject.activeInHierarchy)
        {
            refCamTran.gameObject.SetActive(false);
        }
        this.refCamTran = refCamTran;

        if (!blackCam)
            blackCam = CreateCamera("BlackCameraForRT", Color.black);
        if (!whiteCam)
            whiteCam = CreateCamera("WhiteCameraForRT", new Color(shaderAdditiveOffset / 255f, 1f, 1f, 1f));//Color.white);
        if (!blackRT)
            blackRT = CreateTexture(renderTextureFormat);
        if (!whiteRT)
            whiteRT = CreateTexture(renderTextureFormat);
        blackCam.allowHDR = false;
        whiteCam.allowHDR = false;//有些低端机型不支持HDR，暂时禁用
        blackCam.targetTexture = blackRT;
        whiteCam.targetTexture = whiteRT;

        AddImage(rawImg);
    }

    public void ResetRawImg(RawImage rawImg)
    {
        this.rawImg = rawImg;
        this.AddImage(rawImg);
    }
    public void ResetCamPRS(Transform refTran)
    {
        if (blackCam != null)
        {
            blackCam.transform.localPosition = refTran.localPosition;
            blackCam.transform.localScale = refTran.localScale;
            blackCam.transform.localRotation = refTran.localRotation;
        }
        if (whiteCam != null)
        {
            whiteCam.transform.localPosition = refTran.localPosition;
            whiteCam.transform.localScale = refTran.localScale;
            whiteCam.transform.localRotation = refTran.localRotation;
        }
    }
    public RenderTexture StartRenderRT(Vector2 rtSize, Transform refCamTran, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGBHalf)
    {
        this.rtSize = rtSize;
        if (refCamTran.gameObject.activeInHierarchy)
        {
            refCamTran.gameObject.SetActive(false);
        }
        this.refCamTran = refCamTran;

        if (!blackCam)
            blackCam = CreateCamera("BlackCameraForRT", Color.black);
        if (!whiteCam)
            whiteCam = CreateCamera("WhiteCameraForRT", Color.white);
        if (!blackRT)
            blackRT = CreateTexture(renderTextureFormat);
        if (!whiteRT)
            whiteRT = CreateTexture(renderTextureFormat);

        blackCam.targetTexture = blackRT;
        whiteCam.targetTexture = whiteRT;
        return whiteRT;
    }

    public void StartRenderRawImg(RawImage rawImg)
    {
        if (whiteRT != null)
        {
            this.rawImg = rawImg;
            this.AddImage(rawImg);
        }
    }

    private void Update()
    {
        if (this.rawImg == null)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    protected void OnDestroy()
    {
        if (rawImgMat)
            Destroy(rawImgMat);
        if (blackRT)
        {
            Destroy(blackRT);
            blackRT = null;
        }
        if (whiteRT)
        {
            Destroy(whiteRT);
            whiteRT = null;
        }

        if (blackCam)
            Destroy(blackCam.gameObject);
        if (whiteCam)
            Destroy(whiteCam.gameObject);


    }


    ///// <summary>正交相机大小</summary>
    //public float orthographicSize
    //{
    //    get { return m_OrthographicSize; }
    //    set { ChangeCameraSize(value); }
    //}

    /// <summary>是否灰度化</summary>
    public bool grey
    {
        set
        {
            if (rawImgMat)
                rawImgMat.SetFloat("_Grey", value ? 1 : 0);
        }
    }

    /// <summary>当前位置</summary>
    public Vector3 Center
    {
        get { return transform.position; }
    }

    /// <summary>添加渲染目标</summary>
    public void AddImage(RawImage img)
    {
        if (!img)
            return;
        if (whiteRT == null)
        {
            Debug.LogError("whiteRT is null");
        }
        img.texture = whiteRT;

        if (rawImgMat != null)
        {
            GameObject.Destroy(rawImgMat);
        }

        rawImgMat = CreateMaterial();
        img.material = rawImgMat;
        if (img.gameObject.activeSelf == false)
        {
            img.gameObject.SetActive(true);
        }
        if (img.enabled == false)
        {
            img.enabled = true;
        }
    }
    //TODO:不如直接instantiate目标相机
    private Camera CreateCamera(string name, Color bgColor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(refCamTran.transform.parent);
        obj.transform.localPosition = refCamTran.localPosition;
        obj.transform.localScale = refCamTran.localScale;
        obj.transform.localRotation = refCamTran.localRotation;


        //相机的参数调整成和refCam一致
        var refCam = refCamTran.GetComponent<Camera>();
        var camera = obj.AddComponent<Camera>();
        camera.backgroundColor = bgColor;
        camera.orthographic = refCam.orthographic;
        camera.orthographicSize = refCam.orthographicSize;

        camera.allowMSAA = false;
        camera.allowHDR = false;
        camera.fieldOfView = refCam.fieldOfView;
        camera.clearFlags = CameraClearFlags.Color;
        camera.farClipPlane = refCam.farClipPlane;
        camera.nearClipPlane = refCam.nearClipPlane;
        camera.depth = refCam.depth;
        camera.cullingMask = refCam.cullingMask;
        if (bgColor == Color.black)
        {
            var uacd_ref = refCam.GetComponent<UniversalAdditionalCameraData>();
            var uacd = camera.gameObject.GetOrAddComponent<UniversalAdditionalCameraData>();
            uacd.renderPostProcessing = true;
            uacd.volumeLayerMask = uacd_ref.volumeLayerMask;
        }

        if (refCam.clearFlags == CameraClearFlags.Skybox)
        {
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        return camera;
    }

    private RenderTexture CreateTexture(RenderTextureFormat renderTextureFormat)
    {
        //24bit with stencil
        //otherwise these may be some render order error  with model
        var texture = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, renderTextureFormat)
        {
            antiAliasing = 1,
            filterMode = FilterMode.Bilinear,
            anisoLevel = 0,
            useMipMap = false,
            autoGenerateMips = false,
        };

        texture.name = transform.name + "_RT";
        return texture;
    }

    private Material CreateMaterial()
    {
        Material mat = new Material(Shader.Find("UI/GRenderShader"));
        mat.SetTexture("_MainTex", whiteRT);
        mat.SetTexture("_BlackTex", blackRT);
        return mat;
    }

}
