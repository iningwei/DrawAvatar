using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCameraController : MonoBehaviour
{
    [Header("=== 移动设置 ===")]
    [Tooltip("WASD 移动速度")]
    public float moveSpeed = 8f;

    [Header("=== 鼠标视角设置 ===")]
    [Tooltip("鼠标旋转灵敏度")]
    public float lookSensitivity = 2f;
    [Tooltip("垂直视角上下限制（89度防止翻转）")]
    public float verticalClamp = 89f;

    [Header("=== 滚轮缩放设置 ===")]
    [Tooltip("滚轮缩放速度")]
    public float zoomSpeed = 10f;
    [Tooltip("滚轮缩放平滑值,越大越平滑")]
    public float zoomSmooth = 1f;
    // 内部旋转角度
    private float yaw;    // 左右旋转
    private float pitch;  // 上下旋转

    [Header("=== 平移安全区域,XYWH分别为:Z轴最小和最大值,X轴最小和最大值 ===")]
    [SerializeField]
    public Rect safeArea;
    [Header("=== 高度安全区间,XY分别为:Y轴最小和最大值 ===")]
    public Vector2 safeHeight;

    public static FreeCameraController instance;
    void Start()
    {
        instance = this;
        // 初始化视角
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        cameraTargetHeight = transform.position.y;
    }

    void Update()
    {
        // 1. 鼠标控制360度旋转
        RotateCamera();

        // 2. WASD 平移移动
        MoveCamera();

        // 3. 滚轮缩放
        ZoomCamera();
    }

    // 鼠标旋转相机
    void RotateCamera()
    {
        if (Input.GetMouseButton(1)) // 按住鼠标右键旋转视角
        {
            yaw += Input.GetAxis("Mouse X") * lookSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;

            // 限制上下角度，防止相机翻转
            pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    // WASD 平移相机
    void MoveCamera()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A D
        float v = Input.GetAxisRaw("Vertical");   // W S

        // 基于相机自身方向前后左右移动
        //Vector3 dir = transform.right * h + transform.forward * v;
        //dir.y = 0; // 禁止垂直飞行（想飞就删掉这句）
        //if (dir.magnitude > 1f) dir.Normalize();
        //transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

        var quaternion = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        Vector3 dir = quaternion * Vector3.right * h + quaternion * Vector3.forward * v;
        if (dir.magnitude > 1f) dir.Normalize();
        var newPos = transform.position + dir * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, safeArea.width, safeArea.height);
        newPos.z = Mathf.Clamp(newPos.z, safeArea.x, safeArea.y);
        transform.position = newPos;
    }

    float cameraTargetHeight;
    // 滚轮缩放
    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            //// 相机前后移动实现缩放效果
            //transform.Translate(0, 0, scroll * zoomSpeed, Space.Self);

            cameraTargetHeight -= scroll * zoomSpeed;
            cameraTargetHeight = Mathf.Clamp(cameraTargetHeight, safeHeight.x, safeHeight.y);
        }
        float lerp = Mathf.Lerp(transform.position.y, cameraTargetHeight, Time.deltaTime / zoomSmooth);
        transform.position = new Vector3(transform.position.x, lerp, transform.position.z);
    }
}