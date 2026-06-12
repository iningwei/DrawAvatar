using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using ZGame.Obfuscation;
using System.Linq;
using System;


//GameEntry.asmdef 需要添加对Unity.Addressables 、Unity.ResourcesManager 的引用，Addressables、ResourceManagement 命名空间才可使用
public class HybridCLRAAResourceUpdate : SingletonMonoBehaviour<HybridCLRAAResourceUpdate>
{
    //服务端资源版本号
    public int serverResVer;

    GameEntry gameEntry;

    string serverResRootUrl;
    /// <summary>
    /// 
    /// </summary>
    public void Enter(GameEntry gameEntry, string serverResRootUrl)
    {
        this.gameEntry = gameEntry;
        this.serverResRootUrl = serverResRootUrl;

        //检测资源是否需要更新
        var localResVer = int.Parse(GetLocalResVersion());
        serverResVer = int.Parse(ServerList.Instance.GetAppMaxResVersion(ConfigUtility.Data.AppVersion));

        if (serverResVer > localResVer)
        {
            Debug.Log("res need update,sverResVer:" + serverResVer + ", localResVer：" + localResVer);
            StartCoroutine(AddressableHotupdate2(serverResVer));
        }
        else if (serverResVer == localResVer)
        {
            Debug.Log($"res already update to newest res version:{serverResVer},enterGame！");

            EnterGame();
        }
        else
        {
            //sverver<localver也直接进入游戏
            //造成这种情况的原因，比如某用户通过商店下载个资源版本为10的版本，但是ftp那边配置的最大版本是9，并没有及时切换到10
            Debug.LogWarning("Warning：sverver<localver, sverVer:" + serverResVer + ", localVer:" + localResVer);
            EnterGame();
        }
    }

    private IEnumerator AddressableHotupdate(int resVersion)
    {
        var remoteCatalogUrl = serverResRootUrl + "/" + ConfigUtility.Data.AppVersion + "/" + resVersion + "/catalog.json";
        List<object> keys = new List<object>();// { hotupdateDllName};  
        Debug.Log("初始化成功，准备 加载远程 catalog ：" + remoteCatalogUrl);
        // 加载远程 catalog
        AsyncOperationHandle<IResourceLocator> catalogHandle = Addressables.LoadContentCatalogAsync(remoteCatalogUrl);
        yield return catalogHandle;

        //添加key  
        IResourceLocator locator = catalogHandle.Result;
        foreach (var item in locator.Keys)//locator.Keys里面有很多重复的key，某个资源既有明文key，又有密文key，会导致后续下载出现重复。故这里需要过滤，只保留需要的明文key
        {
            string keyStr = item.ToString();
            if (keyStr.EndsWith(".bytes") ||
                keyStr.EndsWith(".unity") ||
                keyStr.EndsWith(".prefab") ||
                keyStr.EndsWith(".spriteatlas") ||
                keyStr.EndsWith(".asset") ||
                keyStr.EndsWith(".mat") ||
                keyStr.EndsWith(".shadervariants") ||
                keyStr.EndsWith(".shader") ||
                 keyStr.EndsWith(".mp4") ||
                keyStr.EndsWith(".shadergraph"))
            {
                keys.Add(item);
            }
        }


        if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("远程 catalog 加载成功，开始检查更新...");

            // 检查下载大小 ）
            long totalDownloadSize = 0;
            //法一：逐个计算
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(keys[i]);
                yield return sizeHandle;
                if (sizeHandle.Result > 0)
                {
                    Debug.Log(keys[i] + ",  size(byte):" + sizeHandle.Result);
                    totalDownloadSize += sizeHandle.Result;
                }
                else
                {
                    //从keys移除
                    keys.RemoveAt(i);
                }

                Addressables.Release(sizeHandle);
            }

            ////////法二：整体计算
            //////var sizeHandle = Addressables.GetDownloadSizeAsync(keys.AsEnumerable());
            //////yield return sizeHandle;
            //////if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            //////{
            //////    Debug.LogError("GetDownloadSizeAsync Error\n" + sizeHandle.OperationException.ToString());
            //////    yield break;
            //////}
            //////totalDownloadSize = sizeHandle.Result;


            Debug.Log($"需要下载的总大小: {totalDownloadSize / 1024f / 1024f} MB");

            if (totalDownloadSize > 0)
            {
                //法一， 逐个下载所有需要更新的 bundle
                //////foreach (var key in keys)
                //////{
                //////    AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key, false);
                //////    while (!downloadHandle.IsDone)
                //////    {
                //////        Debug.Log($"下载进度 ({key}): {downloadHandle.PercentComplete * 100}%"); 
                //////        yield return null;
                //////    }

                //////    if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
                //////    {
                //////        Debug.LogError($"下载失败 ({key}): " + downloadHandle.OperationException);
                //////        yield break;
                //////    }

                //////    Addressables.Release(downloadHandle);
                //////}

                //法二， 整体下载所有需要更新的bundle 
                var downloadHandle = Addressables.DownloadDependenciesAsync(keys.AsEnumerable(), Addressables.MergeMode.Union, false);//这里要是使用true参数(自动释放handle)，则会报错
                while (!downloadHandle.IsDone)
                {
                    if (downloadHandle.Status == AsyncOperationStatus.Failed)
                    {
                        Debug.LogError("DownloadDependenciesAsync Error\n" + downloadHandle.OperationException.ToString());
                        this.gameEntry.ShowErrorTip("Error AA 1");//showError window更好吧？
                        yield break;
                    }
                    // 下载进度
                    float percentage = downloadHandle.PercentComplete;
                    Debug.Log($"已下载 百分比: {percentage}");
                    this.gameEntry.ShowProgress(percentage);
                    yield return null;
                }
                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("下载完毕!");
                }
                else
                {
                    Debug.LogError($"下载失败  : {downloadHandle.OperationException}");
                    this.gameEntry.ShowErrorTip("Error AA 2");
                    yield break;
                }
                Addressables.Release(downloadHandle);
            }
        }
        else
        {
            Debug.LogWarning("加载远程 catalog 失败，使用本地 bundle: " + catalogHandle.OperationException);
            this.gameEntry.ShowErrorTip("Error AA 3");
            yield break;
        }

        Addressables.Release(catalogHandle);
        //写入最新资源版本号 
        Debug.Log("写入最新资源版本号：" + resVersion);
        SetLocalResVersion(resVersion.ToString());

        this.EnterGame();
    }


    long totalDownloadSize;
    AsyncOperationStatus addressableHotupdateStatus;
    private IEnumerator AddressableHotupdate2(int resVersion)
    {
        List<object> keys = new List<object>();
        totalDownloadSize = 0;

        yield return Addressables.InitializeAsync();

        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        Debug.Log("2 catalogs check result:" + checkHandle.Status);
        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            List<string> catalogs = checkHandle.Result;
            if (catalogs != null && catalogs.Count > 0)
            {
                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
                yield return updateHandle;

                var catalogResult = updateHandle.Result;
                foreach (var item in catalogResult)
                {
                    //Debug.Log("catalog result: " + item.LocatorId);
                    //foreach (var key in item.Keys)
                    //{ 
                    //if (key.ToString().EndsWith(".bundle"))
                    //{
                    //    if (key.ToString().StartsWith("fontgroup"))//字体不检查更新。具体名字根据项目实际情况设置
                    //    {
                    //        continue;
                    //    }
                    //    keys.Add(key);
                    //    Debug.Log("add key:" + key.ToString());
                    //}
                    //}
                    keys.AddRange(item.Keys);
                }

                Debug.Log("updateHandle result:" + updateHandle.Status);
                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    //计算下载文件总大小------------------------>
                    //整体计算要下载文件大小
                    var sizeHandle = Addressables.GetDownloadSizeAsync(keys.AsEnumerable());
                    yield return sizeHandle;
                    if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        totalDownloadSize = sizeHandle.Result;
                        Debug.Log("totalDownloadSize:" + (totalDownloadSize / 1024f / 1024f).ToString("0.0") + "MB");
                        addressableHotupdateStatus = AsyncOperationStatus.Succeeded;
                    }
                    else
                    {
                        addressableHotupdateStatus = AsyncOperationStatus.Failed;
                        this.gameEntry.ShowErrorTip("Error sizeHandle");
                    }
                    Addressables.Release(sizeHandle);
                    //<------------------------


                    //////// 逐个计算------------------------>有bug 会卡住
                    //////bool isSizeCaculateSuccess = true;
                    //////for (int i = keys.Count - 1; i >= 0; i--)
                    //////{
                    //////    AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(keys[i]);
                    //////    yield return sizeHandle;
                    //////    if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
                    //////    {
                    //////        long size = sizeHandle.Result;
                    //////        if (size > 0)
                    //////        {
                    //////            Debug.Log(keys[i] + ", size(Byte)" + size + ",  size(MB):" + (size / 1024f / 1024f).ToString("0.000"));
                    //////            totalDownloadSize += size;
                    //////        }
                    //////        else
                    //////        {
                    //////            //从keys移除
                    //////            keys.RemoveAt(i);
                    //////        }
                    //////    }
                    //////    else
                    //////    {
                    //////        isSizeCaculateSuccess = false;
                    //////        this.gameEntry.ShowErrorTip("Error sizeHandle");
                    //////    }

                    //////    Addressables.Release(sizeHandle);
                    //////    if (isSizeCaculateSuccess == false)
                    //////    {
                    //////        break;
                    //////    }
                    //////}
                    //////if (isSizeCaculateSuccess)
                    //////{
                    //////    Debug.Log("totalDownloadSize, Byte:" + totalDownloadSize + ", MB:" + (totalDownloadSize / 1024f / 1024f).ToString("0.000"));
                    //////    addressableHotupdateStatus = AsyncOperationStatus.Succeeded;
                    //////}
                    //////else
                    //////{
                    //////    addressableHotupdateStatus = AsyncOperationStatus.Failed;
                    //////}
                    ////////<----------------------------
                }
                else
                {
                    addressableHotupdateStatus = AsyncOperationStatus.Failed;
                    this.gameEntry.ShowErrorTip("Error updateHandle");
                }
                Addressables.Release(updateHandle);
            }
            else
            {
                Debug.Log("不需要更新catalog");
                addressableHotupdateStatus = AsyncOperationStatus.Succeeded;
            }
        }
        else
        {
            addressableHotupdateStatus = AsyncOperationStatus.Failed;
            this.gameEntry.ShowErrorTip("Error checkHandle");

        }
        Addressables.Release(checkHandle);


        if (addressableHotupdateStatus == AsyncOperationStatus.Succeeded)
        {
            if (totalDownloadSize > 0)
            {
                //下载热更资源
                var downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(keys.AsEnumerable(), Addressables.MergeMode.Union, false);
                long totalSize = downloadDependenciesHandle.GetDownloadStatus().DownloadedBytes;
                Debug.Log("begin download -->totalSize:" + totalSize + ", mb:" + (totalSize / 1024f / 1024f).ToString("0.000"));

                while (downloadDependenciesHandle.Status == AsyncOperationStatus.None)
                {
                    var percentage = downloadDependenciesHandle.GetDownloadStatus().Percent;
                    Debug.Log("download percentage：" + (percentage * 100).ToString("0.00") + "%");
                    this.gameEntry.ShowProgress(percentage);
                    yield return null;
                }
                if (downloadDependenciesHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("dependencies download success");
                    this.whileAddressableUpdateSuccess(resVersion);
                }
                else
                {
                    Debug.Log("dependencies download failed");
                    this.gameEntry.ShowErrorTip("Error dependenciesHandle");
                }
                Addressables.Release(downloadDependenciesHandle);
            }
            else
            {
                this.whileAddressableUpdateSuccess(resVersion);
            }
        }



    }
    //<-------------------




    //没调通
    private IEnumerator AddressableHotupdate3(int resVersion)
    {
        List<object> keys = new List<object>();
        totalDownloadSize = 0;
        yield return Addressables.InitializeAsync();

        string remoteCatalogUrl = serverResRootUrl + "/" + ConfigUtility.Data.AppVersion + "/catalog_" + ConfigUtility.Data.AppVersion + ".json";
        Debug.Log("remoteCatalogUrl:" + remoteCatalogUrl);
        AsyncOperationHandle<IResourceLocator> checkCatalogHandle = Addressables.LoadContentCatalogAsync(remoteCatalogUrl);
        yield return checkCatalogHandle;
        Debug.Log("3 catalog check result:" + checkCatalogHandle.Status);
        if (checkCatalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IResourceLocator locator = checkCatalogHandle.Result;
            keys.AddRange(locator.Keys);

            //计算下载文件总大小------------------------>
            //整体计算要下载文件大小
            var sizeHandle = Addressables.GetDownloadSizeAsync(keys.AsEnumerable());
            yield return sizeHandle;
            if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                totalDownloadSize = sizeHandle.Result;
                Debug.Log("totalDownloadSize:" + (totalDownloadSize / 1024f / 1024f).ToString("0.0") + "MB");
                addressableHotupdateStatus = AsyncOperationStatus.Succeeded;
            }
            else
            {
                addressableHotupdateStatus = AsyncOperationStatus.Failed;
                this.gameEntry.ShowErrorTip("Error sizeHandle");
            }
            Addressables.Release(sizeHandle);
            //<------------------------ 
        }
        else
        {
            addressableHotupdateStatus = AsyncOperationStatus.Failed;
            this.gameEntry.ShowErrorTip("Error checkCatalogHandle");
        }
        Addressables.Release(checkCatalogHandle);




        //下载
        if (addressableHotupdateStatus == AsyncOperationStatus.Succeeded)
        {
            if (totalDownloadSize > 0)
            {
                //下载热更资源
                var downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(keys.AsEnumerable(), Addressables.MergeMode.Union, false);
                long totalSize = downloadDependenciesHandle.GetDownloadStatus().DownloadedBytes;
                Debug.Log("begin download -->totalSize:" + totalSize + ", mb:" + (totalSize / 1024f / 1024f).ToString("0.000"));

                while (downloadDependenciesHandle.Status == AsyncOperationStatus.None)
                {
                    var percentage = downloadDependenciesHandle.GetDownloadStatus().Percent;
                    Debug.Log("download percentage：" + (percentage * 100).ToString("0.00") + "%");
                    this.gameEntry.ShowProgress(percentage);
                    yield return null;
                }
                if (downloadDependenciesHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("dependencies download success");
                    this.whileAddressableUpdateSuccess(resVersion);
                }
                else
                {
                    Debug.Log("dependencies download failed");
                    this.gameEntry.ShowErrorTip("Error dependenciesHandle");
                }
                Addressables.Release(downloadDependenciesHandle);
            }
            else
            {
                this.whileAddressableUpdateSuccess(resVersion);
            }
        }
    }


    private void whileAddressableUpdateSuccess(int resVersion)
    {
        //写入最新资源版本号 
        Debug.Log("写入最新资源版本号：" + resVersion);
        SetLocalResVersion(resVersion.ToString());
        //进入游戏
        this.EnterGame();
    }
    //<-----------------------------------


    void EnterGame()
    {
        this.gameEntry.EnterGame();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetLocalResVersion()
    {
        //TODO:资源更新的版本号写到PlayerPrefs中，如果用户覆盖安装了一个低版本的呢，会造成什么影响？？


        //先判断playerprefs是否有版本号，如果没有则从包配置里读取
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        Debug.Log("resversion_" + ConfigUtility.Data.AppVersion + "-->" + "localResVersion：" + localResVersion);
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }

        return localResVersion.ToString();
    }

    public void SetLocalResVersion(string version)
    {
        PlayerPrefs.SetString("resversion_" + ConfigUtility.Data.AppVersion, version);
        PlayerPrefs.Save();
    }

}
