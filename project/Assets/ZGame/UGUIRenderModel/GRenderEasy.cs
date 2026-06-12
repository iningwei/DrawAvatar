using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class GRenderEasy : MonoBehaviour
{
    [SerializeField]
    private Vector2 rtSize;
    [SerializeField]
    private RenderTexture rt;
    [SerializeField]
    private RawImage rawImg;

    public Transform refCamTran;


    public void StartRender(Vector2 rtSize, RawImage rawImg, Transform refCamTran, float shaderAdditiveOffset = 255)
    {
        this.rtSize = rtSize;
        this.rawImg = rawImg;
        this.refCamTran = refCamTran;
        if (!rt)
        {
            rt = CreateTexture();
        }
        refCamTran.gameObject.SetActive(true);
        var camera = this.refCamTran.GetComponent<Camera>();
        camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
        camera.targetTexture = rt;
        AddImage(rawImg);
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
        if (rt)
        {
            Destroy(rt);
            rt = null;
        }
    }



    /// <summary>添加渲染目标</summary>
    public void AddImage(RawImage img)
    {
        if (!img)
            return;
        img.texture = rt;
        if (img.gameObject.activeSelf == false)
        {
            img.gameObject.SetActive(true);
        }
        if (img.enabled == false)
        {
            img.enabled = true;
        }
    }

    private RenderTexture CreateTexture()
    {
        //24bit with stencil
        //otherwise these may be some render order error  with model
        var texture = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, RenderTextureFormat.ARGBFloat)
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


}
