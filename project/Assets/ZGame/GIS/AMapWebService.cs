using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.GIS
{
    /// <summary>
    /// 高德地图网络服务
    /// </summary>
    public class AMapWebService
    {
        //地球半径，单位米
        private const double EARTH_RADIUS = 6378137;
        public static string AMapWebKey = "0e3b9e111ac6a2f850619188c52d66ea";
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位：米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <returns></returns>
        public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

        //地理/逆地理编码 https://lbs.amap.com/api/webservice/guide/api/georegeo/
        public static void GetCityCoordinate(string key, string cityName, Action<Vector2> onSuccess)
        {
            //https://restapi.amap.com/v3/geocode/geo?key=0e3b9e111ac6a2f850619188c52d66ea&address=成都
            string url = string.Format("https://restapi.amap.com/v3/geocode/geo?key={0}&address={1}", key, cityName);
            HttpTool.Get(url, IdAssginer.GetID(IdAssginer.IdType.WWW), (id, datas) =>
            {
                var jsonStr = System.Text.Encoding.UTF8.GetString(datas);

                Dictionary<string, object> jsonDic = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
                string status = jsonDic["status"].ToString();
                //获取成功
                if (status == "1")
                {
                    var geoCodes = jsonDic["geocodes"] as List<object>;
                    if (geoCodes != null && geoCodes.Count > 0)
                    {
                        var geoCodeDic = geoCodes[0] as Dictionary<string, object>;
                        string location = geoCodeDic["location"].ToString();
                        string[] strs = location.Split(',');
                        float longitude = float.Parse(strs[0]);
                        float latitude = float.Parse(strs[1]);
                        onSuccess?.Invoke(new Vector2(longitude, latitude));
                    }
                }

            }, (id, error) =>
            {
                Debug.LogError(error);
            }, null, 6);
        }


        //IP定位 https://lbs.amap.com/api/webservice/guide/api/ipconfig/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ip">需要搜索的IP地址（仅支持国内）;若用户不填写IP，则取客户http之中的请求来进行定位</param>
        public static void GetCityNameByIp(string key, string ip, Action<string[]> onSuccess)
        {

            string url = "";
            if (string.IsNullOrEmpty(ip))
            {
                url = string.Format("https://restapi.amap.com/v3/ip?key={0}", key);
            }
            else
            {
                url = string.Format("https://restapi.amap.com/v3/ip?key={0}&ip={1}", key, ip);
            }
            HttpTool.Get(url, IdAssginer.GetID(IdAssginer.IdType.WWW), (id, datas) =>
            {
                var jsonStr = System.Text.Encoding.UTF8.GetString(datas);
                Dictionary<string, object> jsonDic = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
                string status = jsonDic["status"].ToString();
                if (status == "1")
                {
                    string province = "";
                    string city = "";
                    //部分IP如 1.178.208.0 返回的地区数据结构是List<Object>,且列表内无数据，需要做异常处理
                    if (jsonDic["province"].GetType() == typeof(string))
                    {
                        province = jsonDic["province"].ToString();
                        city = jsonDic["city"].ToString();
                    }
                    else
                    {
                        Debug.Log($"非标准地址数据: {jsonDic["province"].GetType()}");
                    }
                    onSuccess?.Invoke(new string[3] {"success", province, city });
                }
                //TODO:
                //status 0表示失败，1表示成功
            }, (id, error) =>
            {
                onSuccess?.Invoke(new string[3] { "fail", "", ""});
                Debug.LogError(error);
            }, null, 3);
        }

    }
}