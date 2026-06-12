using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame
{
    /// <summary>
    /// 本地存储
    /// </summary>
    public class Storage
    {
        /// <summary>
        /// 获得软件的语言设置
        /// </summary>
        /// <returns></returns>
        public static string GetAppLanguage()
        {
            string code = PlayerPrefs.GetString("Language", "");
            if (code == "")
            {
                Debug.Log("Application.systemLanguage:" + Application.systemLanguage);
                if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
                {
                    code = "CN";
                }
                //////else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
                //////{
                //////    code = "TC";
                //////}
                else if (Application.systemLanguage == SystemLanguage.English)
                {
                    code = "EN";
                }
                else if (Application.systemLanguage == SystemLanguage.Japanese)
                {
                    code = "JA";
                }
                else if (Application.systemLanguage == SystemLanguage.Korean)
                {
                    code = "KO";
                }

                else if (Application.systemLanguage == SystemLanguage.Unknown)
                {
                    code = "EN";
                }
                else
                {
                    code = "EN";
                }
                SetAppLanguage(code);
            }

            return code;
        }

        public static void SetAppLanguage(string code)
        {
            PlayerPrefs.SetString("Language", code);
            PlayerPrefs.Save();
            Debug.Log("set language code:" + code);
        }

        public static void SetFCMToken(string token)
        {
            PlayerPrefs.SetString("FCMToken", token);
            PlayerPrefs.Save();
        }
        public static string GetFCMToken()
        {
            return PlayerPrefs.GetString("FCMToken", "");
        }


        public static void SetAudioValue(float v)
        {
            PlayerPrefs.SetFloat("AudioValue", v);
            PlayerPrefs.Save();
        }
        public static float GetAudioValue()
        {
            return PlayerPrefs.GetFloat("AudioValue", 1f);
        }


        public static float GetMusicValue()
        {
            return PlayerPrefs.GetFloat("MusicValue", 1f);
        }
        public static void SetMusicValue(float v)
        {
            PlayerPrefs.SetFloat("MusicValue", v);
            PlayerPrefs.Save();

            EventDispatcher.Instance.DispatchEvent(EventID.OnBgmSliderValueChange, v);
        }




        //音乐状态 开/关
        public static bool GetMusicStatus()
        {
            var value = PlayerPrefs.GetInt("MusicStatus", 1);
            return value == 1;
        }
        public static void SetMusicStatus(bool status)
        {
            PlayerPrefs.SetInt("MusicStatus", status == true ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log("set MusicStatus:" + status);
        }

        //音效状态 开关
        public static bool GetAudioStatus()
        {
            var value = PlayerPrefs.GetInt("AudioStatus", 1);
            return value == 1;
        }
        public static void SetAudioStatus(bool status)
        {
            PlayerPrefs.SetInt("AudioStatus", status == true ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log("set AudioStatus:" + status);
            EventDispatcher.Instance.DispatchEvent(EventID.OnAudioSliderValueChange, status == true ? 1 : 0f);
        }


        public static void SetAppScreenBrightness(int v)
        {
            PlayerPrefs.SetInt("AppScreenBrightness", v);
            PlayerPrefs.Save();
        }

        public static int GetAppScreenBrightness()
        {
            return PlayerPrefs.GetInt("AppScreenBrightness", 190);
        }
    }
}