using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame;
using ZGame.Ress;
using ZGame.Ress.AB;

public class GameBridge
{
    public static void LoadLauncherScene()
    {
        int resLoadType = ConfigUtility.Data.ResLoadType;
        if (resLoadType == (int)ResLoadType.Resources)
        {
            SceneManager.LoadScene("Launcher", LoadSceneMode.Single);
            //这种会出现跳屏
            //原因分析：Launcher以Single加载，则Launcher加载后，GameEntry会卸载。Launcher加载后，再加载LoginWindow也是异步的，这期间Launcher场景下的UICamera是没有UI照射的，导致短时间的蓝屏（SolidColor颜色）

            //为解决跳屏问题，GameEntry场景的Canvas不能设置RenderMode为Screen Space-Overlay，否则其在最高层，会影响后续在Launcher场景期间一些提示信息的展示。请设置为Screen Space-Camera。
            //加载Launcher使用Additive方式，Launcher场景中的UICamera需要保证渲染Priority比GameEntry中的Camera要高（即较后渲染）。
            //加载Launcher时GameEntry还保留的，此时显示的是GameEntry场景内的UI，此时Launcher场景下相机还是Base模式，BackgroundType设置为Uninitialized
            //当在Launcher场景走完LoginWindow加载，进入新游戏场景通过Single加载，再完成对GameEntry的卸载。进入新场景后，新的游戏玩法3D或者2D相机作为Base相机，Launcher的UICamera相机的RenderType调整为Overlay。

            //注意Launcher场景中的UI相机在Base模式下BackgroundType不能设置为SolidColor，需要设置为Uninitialized。若设置为SolidColor的话，还是一样的道理，在Launcher加载的一瞬间，由于UI加载是异步的，可能会有几帧时间内Launcher下无UI渲染，而且上文也提到Launcher场景中UICamera的Priority比Camera高。这样的话会再次导致蓝屏几帧的问题。


            //补充：上述设置Launcher场景中UICamera的BackgroundType为Uninitialized还是不行。在PC浏览器上上述提到的蓝屏阶段会花屏；在移动端浏览器（safari,chrome）没问题。
            //为了解决这个问题--------------------------->
            //GameEntry中相机RenderType设置为Base, Priority设置为-1；Canvas的RenderMode设置为ScreenSpace-Camera
            //Launcher中相机RenderType也设置为Base，初始Priority设置为-2，BackgroundType使用默认的SolidColor；Canvas的RenderMode设置为ScreenSpace-Camera
            //这样可以保证GameEntry->Launcher以Additive加载时不会出现跳屏
            //当Launcher中首窗体加载后，卸载GameEntry场景
        }
        else if (resLoadType == (int)ResLoadType.Addressable)
        {
            AppManager.Instance.RegisterFirstWindowShowAct(() =>
            {
                SceneManager.UnloadSceneAsync("GameEntry");
            });
            AAManager.Instance.LoadScene("Launcher", LoadSceneMode.Additive, null);//由于webgl下AA 全是异步，为了避免 GameEntry->Launcher->主场景 中间出现跳屏，故这里使用Additive，以保证场景加载过程中背景的顺滑。
        }
        else if (resLoadType == (int)ResLoadType.AssetBundle)
        {
            ABManager.Instance.LoadScene("Launcher", UnityEngine.SceneManagement.LoadSceneMode.Single, null, null, true);
        }
    }

    public static void LoadAllTables()
    {
        TableFetch.LoadAllTables();
    }
    public static bool IsAllTablesLoaded()
    {
        return TableFetch.IsAllTablesLoaded();
    }
}
