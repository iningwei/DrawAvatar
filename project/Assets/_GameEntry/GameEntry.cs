#if HybridCLR_INSTALLED
using HybridCLR;
#endif 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEntry : MonoBehaviour
{
    public Transform normalTipTran;
    public Text normalTipTxt;

    public Transform errorTipTran;
    public Text errorTipTxt;

    public Transform progressTran;
    public Image progressImg;
    public Text progressTxt;

    public Transform bigUpdateTran;

    public Image splashImage;
    float splashUsedTime = 0f;
    bool isSplashFinished = false;

    public Text tipDesTxt;
    public Text ratioTxt;

    string hotupdateDllName = "Assembly-CSharp.dll";
    List<string> aotMetaAssemblyFiles;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SetLogTraceType()
    {
        //日志输出设置
        //这个要早于任何其它日志输出前设置，否则都无法生效
        //////#if !UNITY_EDITOR
        //////        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        //////        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        //////        Debug.Log("set statckTraceType");
        //////#endif
    }

    private string serverResRootUrl;
    void Start()
    {

        hotupdateDllName = (ConfigUtility.Data.IsABResNameCrypto ? HybridCLRDES.EncryptStrToHex(hotupdateDllName, ConfigUtility.Data.ABResNameCryptoKey) : hotupdateDllName) + ".bytes";
        Debug.Log("hotupdateDllName:" + hotupdateDllName);
        if (ConfigUtility.Data.IsABResNameCrypto)
        {
            aotMetaAssemblyFiles = (AOTMetaAssembly.GetCryptedBytesNames());
        }
        else
        {
            aotMetaAssemblyFiles = (AOTMetaAssembly.GetOriginBytesNames());
        }
        serverResRootUrl = ConfigUtility.GetPackItem().FtpTxtFileURL + "/" + IOTools.PlatformFolderName + "/channel_" + ConfigUtility.Data.GameChannelId;

        normalTipTran.gameObject.SetActive(false);
        errorTipTran.gameObject.SetActive(false);
        progressTran.gameObject.SetActive(false);
        bigUpdateTran.gameObject.SetActive(false);


        splashImage.gameObject.SetActive(true);
    }


    bool isNetworkReachable = false;
    int checkedCount = 0;

    private void Update()
    {
        splashUsedTime += Time.deltaTime;
        if (splashUsedTime > 3 && isSplashFinished == false)
        {
            isSplashFinished = true;
            splashImage.gameObject.SetActive(false);

            SDKExt.TryWebRequest();
        }
        if (this.isSplashFinished)
        {
            //Debug.Log("checkedCount:" + checkedCount);
            if (checkedCount > 100)
            {
                ShowNormalTip("The internet not connected！");
                return;
            }
            if (isNetworkReachable == false && IsNetworkReachability())
            {
                isNetworkReachable = true;
                startWork();
            }

            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
            {
                if (isNetworkReachable && this.isAllTablesLoaded() && isAllTablesLoadedFlag == false && isShaderTingsLoaded)
                {
                    isAllTablesLoadedFlag = true;
                    Debug.Log("AllTablesLoaded+ShaderThingsLoaded,   begin loadLauncherScene");
                    this.loadLauncherScene();
                }
            }
        }

        this.showTipDesAnim();
    }

    private void showTipDesAnim()
    {
        int tipDesRepeatCount = (int)splashUsedTime;
        if (tipDesRepeatCount % 3 == 0)
        {
            tipDesTxt.text = "资源加载中.";
        }
        else if (tipDesRepeatCount % 3 == 1)
        {
            tipDesTxt.text = "资源加载中..";
        }
        else if (tipDesRepeatCount % 3 == 2)
        {
            tipDesTxt.text = "资源加载中...";
        }
    }

    void startWork()
    {
#if !UNITY_EDITOR
#if UNITY_WEBGL
        StartCoroutine(downloadPreAssets(this.EnterGameWebGL));
#else
         ServerListDownload.Instance.Download(this, serverResRootUrl); 
#endif
#else
        if (ConfigUtility.Data.LoginType == LoginType.PUB)
        {
            ServerListDownload.Instance.Download(this, serverResRootUrl);
        }
        else if (ConfigUtility.Data.LoginType == LoginType.DEV || ConfigUtility.Data.LoginType == LoginType.DEV_GW)
        {
            this.EnterGame();
        }
        else
        {
            Debug.LogError("！！！！！！！ TODO:");
            this.EnterGame();
        }
#endif
    }


    /// <summary>
    /// 网络可达性
    /// </summary>
    /// <returns></returns>
    public bool IsNetworkReachability()
    {
        checkedCount++;
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("cur used：WiFi ");
                return true;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("cur used: CarrierData");//移动网络
                return true;
            default:
                Debug.LogError("no network reachablility");
                return false;
        }
    }


    public void ShowNormalTip(string tip)
    {
        normalTipTran.gameObject.SetActive(false);
        normalTipTran.gameObject.SetActive(true);
        normalTipTxt.text = tip;
    }

    public void ShowErrorTip(string tip)
    {
        errorTipTran.gameObject.SetActive(true);
        errorTipTxt.text = tip;
    }

    public void ShowBigUpdate()
    {
        this.bigUpdateTran.gameObject.SetActive(true);
        Button confirmBtn = this.bigUpdateTran.Find("bg/ConfirmBtn").GetComponent<Button>();
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(this.onBigUpdateConfirmBtnClicked);

    }

    private void onBigUpdateConfirmBtnClicked()
    {
#if !UNITY_EDITOR
#if UNITY_IOS
        SDKExt.openAppStore(ServerList.Instance.IOSJumpStoreKey);
#endif

 
#if UNITY_ANDROID
        SDKExt.openAndroidDownloadPage(ServerList.Instance.AndroidJumpStoreKey);
#endif
#endif 
    }


    public void ShowProgress(float ratio)
    {
        //在ProgressArea中显示
        //////if (progressTran.gameObject.activeInHierarchy == false)
        //////{
        //////    progressTran.gameObject.SetActive(true);
        //////}
        //////progressImg.fillAmount = ratio;
        //////progressTxt.text = (ratio * 100).ToString("0.0") + "%";


        //在TipDesArea区域显示
        this.ratioTxt.text = (ratio * 100).ToString("0.0") + "%";
    }


    public void EnterGame()
    {
        Assembly hotUpdateAss = null;
        // Editor环境下，热更dll已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR && HybridCLR_HOTUPDATE
        Debug.Log("not editor enviroment load hot assembly");
        // 先补充元数据
        loadMetadataForAOTAssemblies();

        //加载热更dll  
        if (ConfigUtility.Data.ResLoadType == ResLoadType.AssetBundle)
        {
            string path = IOTools.GetFilePath(hotupdateDllName);
            //Debug.Log("begin load dll:" + path);
            byte[] dllDatas = loadData(path);
            hotUpdateAss = Assembly.Load(dllDatas);
            this.handleAfterHotupdateDllLoaded(hotUpdateAss);
        }
        else if (ConfigUtility.Data.ResLoadType == ResLoadType.Addressable)
        {
            Debug.Log("begin load aa dll:" + hotupdateDllName);
            try
            {
                AsyncOperationHandle<TextAsset> op = Addressables.LoadAssetAsync<TextAsset>(hotupdateDllName);
                op.Completed += (handle) =>
                {
                    var datas = handle.Result.bytes;
                    //移除字节头
                    var datasFinal = datas.Skip(HybridCLRConfig.abResByteOffset).ToArray();
                    hotUpdateAss = Assembly.Load(datasFinal);

                    this.handleAfterHotupdateDllLoaded(hotUpdateAss);
                    Addressables.Release(op);
                };
            }
            catch (Exception ex)
            {
                Debug.LogError("error while load aa hotupate dll:" + hotupdateDllName + ", " + ex.ToString());
            }

        }
#else
        Debug.Log("editor load hot assembly");
        // Editor下无需加载，直接查找获得HotUpdate程序集
        hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
        this.handleAfterHotupdateDllLoaded(hotUpdateAss);
#endif


    }


    private void handleAfterHotupdateDllLoaded(Assembly hotUpdateAss)
    {
        gameBridgeType = hotUpdateAss.GetType("GameBridge");
        loadLauncherSceneMethod = gameBridgeType.GetMethod("LoadLauncherScene");
        loadAllTablesMethod = gameBridgeType.GetMethod("LoadAllTables");
        isAllTablesLoadedMethod = gameBridgeType.GetMethod("IsAllTablesLoaded");

        //Addressable下配置表由于是异步加载，故考虑先全部加载进内存
        if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
        {
            this.loadAllTables();

#if !UNITY_EDITOR && SHADER_LIST
            //shader先加载到内存，并预热变体
            StartCoroutine(this.loadShaderThings());
#else
            this.isShaderTingsLoaded = true;
#endif
        }
        else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
        {
            this.loadLauncherScene();
        }
    }

    bool isShaderTingsLoaded = false;
    private IEnumerator loadShaderThings()
    {
        bool shaderListFlag = false;
        bool shaderVariantsFlag = false;
        //加载ShaderList
        AsyncOperationHandle<GameObject> shaderListHandle = Addressables.LoadAssetAsync<GameObject>("ShaderList.prefab");
        yield return shaderListHandle;

        if (shaderListHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var prefabObj = shaderListHandle.Result;
            Debug.Log("ShaderList 加载完成！");
            shaderListFlag = true;
        }
        else
        {
            Debug.LogError("加载ShaderList失败: " + shaderListHandle.OperationException);
        }
        Addressables.Release(shaderListHandle);



        // 加载 Shader Variants（确保已缓存或使用本地版本）
        AsyncOperationHandle<ShaderVariantCollection> shaderHandle = Addressables.LoadAssetAsync<ShaderVariantCollection>("ShaderVariants.shadervariants");
        yield return shaderHandle;

        if (shaderHandle.Status == AsyncOperationStatus.Succeeded)
        {
            ShaderVariantCollection shaders = shaderHandle.Result;
            shaders.WarmUp(); // 预热 Shader，减少运行时卡顿
            Debug.Log("ShaderVariants 加载并预热完成！");
            shaderVariantsFlag = true;
        }
        else
        {
            Debug.LogError("加载 ShaderVariants 失败: " + shaderHandle.OperationException);
        }

        Addressables.Release(shaderHandle);
        if (shaderVariantsFlag && shaderListFlag)
        {
            this.isShaderTingsLoaded = true;
        }
    }

    public void EnterGameWebGL()
    {
#if  !UNITY_EDITOR && HybridCLR_HOTUPDATE
        Debug.Log("not editor enviroment load hot assembly");
        // 先补充元数据
        loadMetadataForAOTAssembliesForWebGL();

        //加载热更dll 
        Assembly hotUpdateAss = Assembly.Load(readDllRealDatasFromStreamingAssets(hotupdateDllName));
#else
        Debug.Log("editor load hot assembly");
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
#endif

        gameBridgeType = hotUpdateAss.GetType("GameBridge");
        loadLauncherSceneMethod = gameBridgeType.GetMethod("LoadLauncherScene");
        loadAllTablesMethod = gameBridgeType.GetMethod("LoadAllTables");
        isAllTablesLoadedMethod = gameBridgeType.GetMethod("IsAllTablesLoaded");

        this.loadAllTables();
    }

    Type gameBridgeType;
    MethodInfo loadLauncherSceneMethod;
    MethodInfo loadAllTablesMethod;
    MethodInfo isAllTablesLoadedMethod;
    void loadLauncherScene()
    {
        loadLauncherSceneMethod?.Invoke(null, null);
    }
    void loadAllTables()
    {
        loadAllTablesMethod?.Invoke(null, null);
    }
    bool isAllTablesLoadedFlag = false;
    bool isAllTablesLoaded()
    {
        if (isAllTablesLoadedMethod == null)
        {
            return false;
        }
        return (Boolean)isAllTablesLoadedMethod?.Invoke(null, null);
    }

#if HybridCLR_HOTUPDATE

    #region webgl相关的处理
    private Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public byte[] readDllRealDatasFromStreamingAssets(string dllName)
    {
        return s_assetDatas[dllName].Skip(HybridCLRConfig.abResByteOffset).ToArray();
    }

    private string getWebRequestPath(string asset)
    {
        var path = $"{Application.streamingAssetsPath}/{asset}";
        if (!path.Contains("://"))
        {
            path = "file://" + path;
        }
        return path;
    }

    IEnumerator downloadPreAssets(Action onDownloadComplete)
    {
        var assets = new List<string>
        {
            hotupdateDllName,//热更代码
              //"prefabs",//一些预加载的预制件
        };

        //元数据dll 
        assets.AddRange(aotMetaAssemblyFiles);


        foreach (var asset in assets)
        {
            string dllPath = getWebRequestPath(asset);
            //Debug.Log($"start download asset:{dllPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllPath);
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            { 
                Debug.Log(www.error);
            }
#endif
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                //Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;
            }
        }

        onDownloadComplete?.Invoke();
    }



    #endregion

     
    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private void loadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误 
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            string fullPath = $"{Application.streamingAssetsPath}/{aotDllName}";

            var dllBytes = loadData(fullPath);
            if (dllBytes == null)
            {
                continue;
            }
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
        }
    }


    void loadMetadataForAOTAssembliesForWebGL()
    {
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            byte[] dllBytes = readDllRealDatasFromStreamingAssets(aotDllName);

            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            //Debug.Log($"LoadMetadataForAOTAssembliesForWebGL:{aotDllName}. ret:{err}");
        }
    }

    private static byte[] loadData(string fullPath)
    {
        string androidSymbol = ".apk!/assets/";
        byte[] dllBytes;
        if (fullPath.Contains(androidSymbol))
        {
            int index = fullPath.IndexOf(androidSymbol);
            string loaderUsedPath = fullPath.Substring(index + androidSymbol.Length);
            if (!HybridCLRIOAndroidLoader.Instance.IsFileExist(loaderUsedPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return null;
            }
            dllBytes = HybridCLRIOAndroidLoader.Instance.GetBytes(loaderUsedPath);
        }
        else
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return null;
            }

            dllBytes = File.ReadAllBytes(fullPath);
        }

        //移除字节头
        return dllBytes.Skip(HybridCLRConfig.abResByteOffset).ToArray();
    }




#endif
}
