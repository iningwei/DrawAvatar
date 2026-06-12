using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ZGame.Window;
using UnityEngine.EventSystems;
using ZGame;
using System;



public class GameCameraControl : MonoBehaviour
{
    [Header("=== 核心设置 ===")]
    [Tooltip("场景中的锚点物体（所有相机逻辑都围绕它）")]
    public Transform anchorTran;

    [Header("=== 距离控制（鼠标滚轮） ===")]
    [Tooltip("最小距离（太近会穿模）")]
    public float minDistance = 15f;
    [Tooltip("最大距离")]
    public float maxDistance = 500f;
    [Tooltip("滚轮缩放速度，数值越大缩放越快")]
    public float zoomSpeed = 80f;

    [Header("=== 旋转控制（左键拖拽） ===")]
    [Tooltip("旋转灵敏度，数值越大转得越快（推荐3-8，5最舒服）")]
    public float rotateSensitivity = 3f;
    [Tooltip("竖直方向最小角度（负数允许抬头，正数强制俯视）")]
    public float minPitch = 10f;
    [Tooltip("竖直方向最大角度（80左右避免正下方穿地）")]
    public float maxPitch = 80f;


    [Header("=== 平移控制（滚轮键按下拖拽） ===")]
    [Tooltip("平移灵敏度，数值越大移动越快（推荐15-30）")]
    public float panSensitivity = 100f;


    [Header("=== 平滑设置（轻微平滑滑动） ===")]
    [Tooltip("旋转平滑时间（0=瞬间，0.08~0.15最自然舒适）")]
    public float rotationSmoothTime = 0.08f;
    [Tooltip("缩放平滑时间（0=瞬间，0.1~0.2最舒服）")]
    public float zoomSmoothTime = 0.12f;
    [Tooltip("平移平滑时间（0=瞬间，0.08~0.15最自然）")]
    public float panSmoothTime = 0.1f;



    [Header("=== 复原设置 ===")]
    [Tooltip("按下 M 键是否同时复原锚点位置")]
    public bool resetAnchorPositionOnM = true;

    // 初始状态记录（Start 时自动保存）
    private float initialDistance;
    private float initialYaw;
    private float initialPitch;
    private Vector3 initialAnchorPosition;

    [Header("=== 内部状态，供调试用，不必设置 ===")]
    public float currentYaw = 0f;
    public float currentPitch = 30f;
    public float currentDistance;

    // 平滑目标值
    private float targetYaw;
    private float targetPitch;
    private float targetDistance;
    private Vector3 targetAnchorPosition;

    // SmoothDamp 所需的速度缓存
    private float yawVelocity = 0f;
    private float pitchVelocity = 0f;
    private float distanceVelocity = 0f;
    private Vector3 anchorVelocity = Vector3.zero;


    public static GameCameraControl instance;
    private void Start()
    {
        instance = this;
        //this.init();
    }

    public void SetAnchor(Transform anchor)
    {
        this.anchorTran = anchor;

        this.init();
    }

    private void init()
    {
        if (anchorTran == null)
        {
            Debug.LogError("未指定 Anchor Tran！");
            return;
        }


        // 自动读取当前相机位置，初始化角度和距离，保证挂上去就和编辑器里看到的完全一致
        currentDistance = Vector3.Distance(transform.position, anchorTran.position);
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;// 防止欧拉角跳到360+，让pitch保持在-180~180区间（更自然）

        // 初始化目标值（与当前值一致）
        targetYaw = currentYaw;
        targetPitch = currentPitch;
        targetDistance = currentDistance;
        targetAnchorPosition = anchorTran.position;

        // ★★★ 记录初始状态，用于 M 键复原 ★★★
        initialDistance = currentDistance;
        initialYaw = currentYaw;
        initialPitch = currentPitch;
        initialAnchorPosition = anchorTran.position;

        Debug.Log("【OrbitalCamera】初始状态已记录，按 M 键可一键复原");
    }

    bool rotateFlag = false;
    bool panFlag = false;
    private void Update()
    {
        if (anchorTran == null) return;


        // 1. 左键拖拽 → 绕锚点旋转（水平无限，竖直限制）
        if (Input.GetMouseButtonDown(0))
        {
            if (UGUIUtility.IsPointerOverUI())
            {
                rotateFlag = false;
            }
            else
            {
                rotateFlag = true;
            }
            return;//第一帧返回，防止PC软件丢失焦点后，再获得焦点，接受第一次点击，后续 Input.GetAxis()获得值,导致相机旋转突变！
        }
        if (Input.GetMouseButtonUp(0))
        {
            rotateFlag = false;
        }

        if (rotateFlag && Input.GetMouseButton(0))
        {
            targetYaw += Input.GetAxis("Mouse X") * rotateSensitivity;
            targetPitch -= Input.GetAxis("Mouse Y") * rotateSensitivity; // 鼠标向上拖 → 抬头（pitch减小） 
            targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        }

        // 2. 鼠标滚轮 → 缩放距离
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0f)
        {
            targetDistance -= scrollDelta * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        // 3. 滚轮键（中键）按下拖拽 → 平移锚点（仅X/Z轴，不影响Y）
        if (Input.GetMouseButtonDown(2))
        {
            panFlag = true;
            return; //第一帧返回
        }
        if (Input.GetMouseButtonUp(2))
        {
            panFlag = false;
        }
        if (panFlag && Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 使用当前Yaw构造水平旋转矩阵，保证平移方向永远和相机视角对齐
            Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);
            Vector3 panMove = yawRotation * new Vector3(mouseX, 0f, mouseY) * panSensitivity * Time.deltaTime;

            targetAnchorPosition += new Vector3(panMove.x, 0, panMove.z);//Y不变
        }

        // ★★★ 新增：按下 M 键一键复原到初始状态 ★★★
        if (Input.GetKeyDown(KeyCode.M))
        {
            targetDistance = initialDistance;
            targetYaw = initialYaw;
            targetPitch = initialPitch;

            if (resetAnchorPositionOnM)
            {
                targetAnchorPosition = initialAnchorPosition;
            }

            Debug.Log("【OrbitalCamera】已复原到初始状态");
        }

        //应用平滑
        currentYaw = Mathf.SmoothDamp(currentYaw, targetYaw, ref yawVelocity, rotationSmoothTime);
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, rotationSmoothTime);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, zoomSmoothTime);

        // 平移锚点也平滑（即使不拖拽也会自然停止）
        anchorTran.position = Vector3.SmoothDamp(anchorTran.position, targetAnchorPosition, ref anchorVelocity, panSmoothTime);

    }

    private void LateUpdate()
    {
        if (anchorTran == null) return;

        // 核心逻辑：根据当前角度和距离重新计算相机位置 + LookAt
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -currentDistance);

        transform.position = anchorTran.position + offset;
        transform.LookAt(anchorTran.position);
    }

    public bool IsPointerOverUI()
    {
        bool pointerOverUI = false;
#if UNITY_EDITOR
        pointerOverUI = EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount > 0)
                {
                    pointerOverUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                }
#endif
        return pointerOverUI;
    }
}
