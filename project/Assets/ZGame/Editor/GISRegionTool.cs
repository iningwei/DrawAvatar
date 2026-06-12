using Codice.Client.Common;
using BestHTTP.JSON.LitJson;
using PlasticPipe.PlasticProtocol.Client.Proxies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using ZGame.GIS;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    public class GISRegionTool : EditorWindow
    {
        static Region region;
        [MenuItem("工具/更新地区经纬度")]
        static void showWindow() {
            region = ReadLocalDate("address_china");
            for (int i = 0; i < region.address.Count; i++)
            {
                Province province = region.address[i];
                count += region.address[i].city.Count;
            }
            Debug.LogError($"开始更新，一共{count}条数据！");
            EditorWindow.GetWindow(typeof(GISRegionTool));
        }

        static bool reqNext = true;
        static int curIndex = 0;
        static int count = 0;
        static void UpdateRegionGIS()
        {
            
            for (int i = 0; i < region.address.Count; i++)
            {
                Province province = region.address[i];  
                for (int j = 0; j < province.city.Count; j++)
                {
                    City city = province.city[j];
                    if (city.lat == 0)
                    {
                        reqNext = false;
                        string adressName = $"{province.name}省{city.name}";
                        AMapWebService.GetCityCoordinate(AMapWebService.AMapWebKey, adressName, (Vector2 v2) => {
                            city.lng = v2.x;
                            city.lat = v2.y;
                            reqNext = true;
                            curIndex++;
                            Debug.Log($"总数：{count} 已更新：{curIndex} {adressName} {v2.x} {v2.y}");
                        });
                        return;
                    }
                }
            }
            reqNext = false;
            Debug.LogError("经纬度更新完成！");
            SaveData();
        }

        int interval = 0;
        private void Update()
        {
            interval++;
            if (interval>15)
            {
                interval = 0;
                if (reqNext)
                {
                    UpdateRegionGIS();
                }
            }

        }
        /// <summary>
        /// 读取本地数据
        /// </summary>
        /// <param name="filename">Resources文件路径</param>
        static Region ReadLocalDate(string filename)
        {
            Region read_region = new Region();
            TextAsset region_data = Resources.Load(filename) as TextAsset;
            if (!region_data)
            {
                Debug.Log("找不到本地数据文件！");
            }
            else
            {
                string text_data = region_data.text;
                read_region = JsonMapper.ToObject<Region>(text_data);
                Debug.Log("解析的本地数据：" + text_data);
            }
            return read_region;
        }

        static void SaveData() {
            
            string jsonStr = JsonMapper.ToJson(region);
            //litJson是Unicode的编码格式，出来的中文乱码，这里把乱码转回去
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            jsonStr = reg.Replace(jsonStr, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
            string path = Application.dataPath + "/Third/ReginSelect/Resources/address_china.json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.Create);  
            StreamWriter sw = new StreamWriter(fs,Encoding.Default);
            sw.Write(jsonStr);
            sw.Close();
            fs.Close();
            //File.WriteAllText(path, jsonStr, System.Text.Encoding.UTF8);
            
            Debug.LogError($"文件写入完成：{path}");
        }
        [MenuItem("工具/定位测试")]
        static void Test() {
            AMapWebService.GetCityNameByIp(AMapWebService.AMapWebKey, "", (string[] adress) => {
                Debug.Log($"{adress[0]} {adress[1]}");
            });
        }
    }
}