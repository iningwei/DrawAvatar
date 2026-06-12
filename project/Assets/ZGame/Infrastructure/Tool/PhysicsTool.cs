using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZGame
{
    public class PhysicsTool
    {
        //封装一层，主要供lua侧调用
        //因为Raycast的重载函数太多，导致lua侧弱类型会调用到其它函数
        //参考：https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/faq.md
        public static bool Raycast(Ray ray, out RaycastHit hit, float maxDis, int layerMask)
        {
            return Physics.Raycast(ray, out hit, maxDis, layerMask);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            return Physics.SphereCast(origin, radius, direction, out hitInfo, maxDistance);
        }

        public static RaycastHit? RaycastByCamAndMousePos(Camera cam, Vector3 mousePos, float dis, int layerMask)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
#if UNITY_EDITOR
            //Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);
            Debug.DrawLine(ray.origin, ray.direction.normalized * dis, Color.red, 0.5f);
#endif
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, dis, layerMask))
            {
                return hit;
            }
            return null;
        }

        /// <summary>
        /// 检测鼠标是否点击到目标物
        /// </summary>
        /// <param name="target"></param>
        /// <param name="camera"></param>
        /// <param name="length"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static bool IsMouseRaycastHitTarget(GameObject target, Camera camera, int length, int layerMask)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, length, layerMask))
            {
                if (hit.collider.gameObject == target)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckBoxCollider(BoxCollider targetBoxArea, int layerMask)
        {
            Vector3 center = targetBoxArea.transform.position + targetBoxArea.center.MultiplyVector3(targetBoxArea.transform.lossyScale);
            Vector3 halfExtents = targetBoxArea.size.MultiplyVector3(targetBoxArea.transform.lossyScale) * 0.5f;
            Quaternion oritation = targetBoxArea.transform.rotation;
            return Physics.CheckBox(center, halfExtents, oritation, layerMask);
        }






        /// <summary>
        /// 排除UI干扰后，判断是否点击到了物体
        /// 常用于射线检测是否点击到物体上
        /// 本函数判断依据：鼠标按下和松开都不能在UI上，否则返回null；鼠标按下和松开是同一个物体，否则返回null
        /// </summary>
        /// <returns></returns>
        public static GameObject GetClickedTargetExcludeUIInterference(Func<bool> conditionFun, Camera cam, float dis, int layerMask, ref GameObject mouseDownTarget, ref GameObject mouseUpTarget)
        {
            bool isPassedCondition = (conditionFun == null || conditionFun() == true);
            if (isPassedCondition)
            {
                #region 获得鼠标按下时的目标
#if UNITY_STANDALONE || UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    mouseDownTarget = null;
                    mouseUpTarget = null;//鼠标按下的时候设置两个变量为null

                    if (EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        mouseDownTarget = getMouseTarget(cam, dis, layerMask);

                    }

                }
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
            mouseDownTarget = null;
            mouseUpTarget = null;//鼠标按下的时候设置两个变量为null
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)==false)
           {
                mouseDownTarget = getMouseTarget(  cam, dis, layerMask);
                 
           }
           
    }
#endif
                #endregion

                #region 获得鼠标松开时目标 , 鼠标松开时返回点击的目标
#if UNITY_STANDALONE || UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        mouseUpTarget = getMouseTarget(cam, dis, layerMask);
                    }


                    if (mouseDownTarget != null && mouseUpTarget != null && mouseDownTarget.Equals(mouseUpTarget))
                    {
                        mouseDownTarget = null;//返回的时候要设置mouseDownTarget为null，避免程序内在update调用这个方法，在获得点击对象后，后续帧持续获得之前缓存的点击对象，这里设置为null，后续帧若没有点击则获取到的为null
                        return mouseUpTarget;
                    }
                }
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)==false)
            { 
                    mouseUpTarget = getMouseTarget(cam, dis, layerMask);
            }

              if (mouseDownTarget != null && mouseUpTarget != null && mouseDownTarget.Equals(mouseUpTarget))
                {
                    mouseDownTarget = null;
                    return mouseUpTarget;
                }
    }
#endif
                #endregion


            }

            return null;
        }


        static GameObject getMouseTarget(Camera cam, float dis, int layerMask)
        {
            var mousePos = Input.mousePosition;
            RaycastHit? hit = RaycastByCamAndMousePos(cam, mousePos, dis, layerMask);
            if (hit != null)
            {
                return hit.Value.collider.gameObject;
            }

            return null;
        }
    }
}
