using System; 
using UnityEngine;
using ZGame;
using ZTools;
/// <summary>
/// 自由扩展
/// </summary>
public class ZCalendarHelper : MonoBehaviour
{
    [Header("Debug模式下自动初始化,仅编辑器中有效！")]
    public bool DebugMode = false;
    public ZCalendar zCalendar;
    private string selectedTimeStr;
    void Start()
    {
#if UNITY_EDITOR
        if (DebugMode)
        {
            zCalendar.Init();
        }
#endif
        zCalendar.UpdateDateEvent += ZCalendar_UpdateDateEvent;
        zCalendar.ChoiceDayEvent += ZCalendar_ChoiceDayEvent;
        zCalendar.RangeTimeEvent += ZCalendar_RangeTimeEvent;
        zCalendar.CompleteEvent += ZCalendar_CompleteEvent;
        //zCalendar.Init();
        //zCalendar.Init(System.DateTime.Now);
        //zCalendar.Init("2022-02-02");
        //zCalendar.Show();
        //zCalendar.Hide();
    }
    /// <summary>
    /// 加载结束
    /// </summary>
    private void ZCalendar_CompleteEvent()
    {
        Debug.Log("ZCalendar加载结束");
        if (null != zCalendar.CrtTime)
        {
            Debug.Log($"当前时间{zCalendar.CrtTime.Day}");
        }
    }

    /// <summary>
    /// 区间时间
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void ZCalendar_RangeTimeEvent(ZCalendarDayItem arg1, ZCalendarDayItem arg2)
    {
        Debug.Log($"选择的时间区间：{arg1.Day}到{arg2.Day}");
    }

    /// <summary>
    /// 获取选择的日期
    /// </summary>
    /// <param name="obj"></param>
    private void ZCalendar_ChoiceDayEvent(ZCalendarDayItem obj)
    {
        Debug.Log($"选择的日期：{obj.Year},{obj.Month},{obj.Day}");
        selectedTimeStr = $"{obj.Year}-{obj.Month}-{obj.Day}";
        _onSelected?.Invoke(selectedTimeStr);
    }

    /// <summary>
    /// 切换月份时，可拿到每一天的item对象
    /// </summary>
    /// <param name="obj"></param>
    private void ZCalendar_UpdateDateEvent(ZCalendarDayItem obj)
    {
        //Debug.Log($"加载日期：{obj.Day}");
    }
    private void OnDestroy()
    {
        zCalendar.UpdateDateEvent -= ZCalendar_UpdateDateEvent;
        zCalendar.ChoiceDayEvent -= ZCalendar_ChoiceDayEvent;
        zCalendar.RangeTimeEvent -= ZCalendar_RangeTimeEvent;
        zCalendar.CompleteEvent -= ZCalendar_CompleteEvent;
    }

    /// <summary>
    /// 获取选中日期的时间戳
    /// </summary>
    /// <returns></returns>
    public string GetSelectedTimeStr()
    {
        return selectedTimeStr;
    }
    private event Action<string> _onSelected;
    /// <summary>
    /// 根据指定时间初始化
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    public void InitDateByTime(string timeStr, Action<string> onSelected)
    {
        _onSelected += onSelected;
        selectedTimeStr = timeStr;
        zCalendar.Init(timeStr);
        //避免编辑器模式下误点debug，造成二次初始化的问题
        DebugMode = false;
    }
}
