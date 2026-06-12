using System.Collections.Generic;

#region 地区
/// <summary>
/// 地区
/// </summary>
public class Region
{
    public List<Province> address;
}

/// <summary>
/// 省
/// </summary>
public class Province
{
    public string name;
    public List<City> city;
}
/// <summary>
/// 市
/// </summary>
public class City
{
    public string name;
    public List<string> area;// 县（区）
    /// <summary>
    /// 经度
    /// </summary>
    public double lng;
    /// <summary>
    /// 纬度
    /// </summary>
    public double lat;
}

#endregion

/// <summary>
/// 地区类型
/// </summary>
public enum RegionType
{
    Null,
    /// <summary>
    /// 省
    /// </summary>
    Province_Type,
    /// <summary>
    /// 市
    /// </summary>
    City_Type,
    /// <summary>
    /// 县（区）
    /// </summary>
    Area_Type
}
