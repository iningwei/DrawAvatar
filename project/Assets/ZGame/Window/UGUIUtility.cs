using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZGame.Window
{
    public class UGUIUtility
    {

        /// <summary>
        /// 世界坐标转换到UGUI坐标
        /// </summary>
        /// <param name="worldCam"></param>
        /// <param name="worldPos"></param>
        /// <param name="rootCanvas"></param>
        /// <returns></returns>
        public static Vector3 WorldToUGUIPos(Camera worldCam, Vector3 worldPos, Canvas rootCanvas)
        {
            Vector2 resultPos = Vector2.zero;
            Vector3 screenPos = worldCam.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.GetComponent<RectTransform>(),
                screenPos,
                rootCanvas.worldCamera,
                out resultPos);

            return new Vector3(resultPos.x, resultPos.y, 0);
        }

        //法2 待测试
        //public static Vector3 WorldToUI(Camera camera, Vector3 pos)
        //{
        //    CanvasScaler scaler = GameObject.Find("UIRoot").GetComponent<CanvasScaler>();

        //    float resolutionX = scaler.referenceResolution.x;
        //    float resolutionY = scaler.referenceResolution.y;

        //    Vector3 viewportPos = camera.WorldToViewportPoint(pos);

        //    Vector3 uiPos = new Vector3(viewportPos.x * resolutionX - resolutionX * 0.5f,
        //        viewportPos.y * resolutionY - resolutionY * 0.5f, 0);

        //    return uiPos;
        //} 


        /// <summary>
        /// 屏幕坐标转换到UGUI坐标
        /// </summary>
        /// <param name="worldCam"></param>
        /// <param name="worldPos"></param>
        /// <param name="rootCanvas"></param>
        /// <returns></returns>
        public static Vector3 ScreenToUGUIPos(Camera worldCam, Vector3 screenPos, Canvas rootCanvas)
        {
            Vector2 resultPos = Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.GetComponent<RectTransform>(),
                screenPos,
                rootCanvas.worldCamera,
                out resultPos);

            return new Vector3(resultPos.x, resultPos.y, 0);
        }

        /// <summary>
        /// 获得UI相对其根节点Canvas的坐标
        /// </summary>
        /// <param name="uiTran"></param>
        /// <param name="rootCanvas"></param>
        /// <returns></returns>
        public static Vector3 GetUIPosRelativeRoot(Transform uiTran, Canvas rootCanvas)
        {
            Vector2 resultPos = Vector2.zero;
            Camera uiCam = rootCanvas.worldCamera;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCam, uiTran.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.GetComponent<RectTransform>(),
                screenPos,
                uiCam,
                out resultPos);
            return new Vector3(resultPos.x, resultPos.y, 0);
        }


        /// <summary>
        /// 根据相对于节点Canvas的坐标rootPos，获得该坐标在某UI节点下的坐标值
        /// </summary>
        /// <param name="rootCanvas"></param>
        /// <param name="rootPos"></param>
        /// <param name="targetUITran"></param>
        /// <returns></returns>
        public static Vector3 GetRelativePosOfRootPosInUISpace(Canvas rootCanvas, Vector3 rootPos, Transform targetUITran)
        {
            Vector3 uiRootPos = GetUIPosRelativeRoot(targetUITran, rootCanvas);

            Vector3 relativePos = new Vector3(rootPos.x - uiRootPos.x, rootPos.y - uiRootPos.y, rootPos.z - uiRootPos.z);


            return relativePos;
        }


        /// <summary>
        /// 当前触摸位置是否在目标UI区域
        /// </summary>
        /// <param name="rootCanvas"></param>
        /// <param name="targetUI"></param>
        /// <returns></returns>
        public static bool IsPointerInRect(Canvas rootCanvas, RectTransform targetUI)
        {
            Camera uiCam = rootCanvas.worldCamera;

            Vector2 screenPos = Vector2.zero;
#if UNITY_EDITOR
            screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#elif UNITY_ANDROID || UNITY_IOS
            screenPos = new Vector2(Input.touchCount > 0 ? Input.GetTouch(0).position.x : 0, Input.touchCount > 0 ? Input.GetTouch(0).position.y : 0);
#endif

            bool isIn = RectTransformUtility.RectangleContainsScreenPoint(targetUI, screenPos, uiCam);

            return isIn;
        }







        /// <summary>
        /// 是否点中了UI
        /// (对于真机的话，当手指点击在UI上，TouchPhase.Ended的时候：EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);返回的为false。
        /// 这对于一些需要通过MouseUp(亦即移动设备上的TouchPhase.Ended)生效的逻辑来说就会有问题，
        /// 因此该方法在按下时可用，松开时慎用）
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUI()
        {
            bool pointerOverUI = false;
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                pointerOverUI = EventSystem.current == null ? false : EventSystem.current.IsPointerOverGameObject();
            }
#elif UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount > 0)
                {
                    pointerOverUI = EventSystem.current == null ? false :  EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    
                }
#endif
            return pointerOverUI;
        }



        //////        bool isPointerOverUI()
        //////        {
        //////            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //////            eventDataCurrentPosition.position = new Vector2
        //////                (
        //////#if UNITY_EDITOR
        //////                Input.mousePosition.x, Input.mousePosition.y
        //////#elif UNITY_ANDROID || UNITY_IOS
        //////           Input.touchCount > 0 ? Input.GetTouch(0).position.x : 0, Input.touchCount > 0 ? Input.GetTouch(0).position.y : 0
        //////#endif
        //////                );
        //////            List<RaycastResult> results = new List<RaycastResult>();
        //////            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //////            if (results.Count > 0)
        //////            {
        //////                GameObject obj = results[0].gameObject;
        //////                if (obj.layer == LayerIndexConfig.UILayer)
        //////                {
        //////                    return true;
        //////                }
        //////            }
        //////            return false;
        //////        }





        static bool validInput = true;
        /// <summary>
        /// 检测点击没有发生在UI上
        /// 返回true表示点击没有发生在UI上
        /// 返回false表名点击发生在UI上
        /// ------------->
        /// 该方法缺点是未记录鼠标按下的对象，鼠标松开时可能不是在按下时的对象上了
        /// 这样的话在调用该方法后，再使用诸如射线检测，若射线检测生效。那么会给人一种困惑。且和常规的点击操作生效逻辑不一致。
        /// 在 PhysicsTool.cs中已实现一个较完善的方法GetClickedTargetExcludeUIInterference()
        /// </summary>
        /// <returns></returns>
        public static bool CheckClickedNotOverUI()
        {
            validateInput();
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonUp(0) && validInput)
            {
                return true;
            }
#else 
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && validInput)
    {
        return true;
    }
#endif
            return false;
        }
        static void validateInput()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    validInput = false;
                else
                    validInput = true;
            }
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
           validInput = false;
        else
            validInput = true;
    }
#endif
        }

    }
}