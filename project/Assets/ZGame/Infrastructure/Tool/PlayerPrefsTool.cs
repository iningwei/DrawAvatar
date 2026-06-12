using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ZGame
{
    /// <summary>
    /// TODO: 轻量级数据持久化统一用这个，后续看有加密需求直接本文件内修改
    /// </summary>
    public static class PlayerPrefsTool
    {
        public static void SetIntArray(string key, int[] array) 
        {
            if (array != null)
            {
               
                string str = "";
                for (int i = 0; i < array.Length; i++)
                {
                    str = $"{str}-{array[i]}";
                }
                PlayerPrefs.SetString(key, str);
            }
            else
            {
                Debug.LogError("PlayerPrefs intArray is Null!");
            }
        }

        public static int[] GetIntArray(string key, int[] defauleValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string str = PlayerPrefs.GetString(key, "");
                string[] strArray = str.Split("-",StringSplitOptions.RemoveEmptyEntries);
                if (strArray != null && strArray.Length > 0)
                {
                    return strArray.ToIntArray();
                }
            }
            return defauleValue;
        }

        public static void SetString(string key, string value)
        {
            if (value != null)
            {
                PlayerPrefs.SetString(key, value);
            }
            else
            {
                PlayerPrefs.SetString(key, "");
            }
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        /// <summary>
        /// Helper method to store a bool in PlayerPrefs (stored as an int)
        /// </summary>
        public static void SetBool(string key, bool value)
        {
            // Store the bool as an int (1 for true, 0 for false)
            if (value)
            {
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }

        /// <summary>
        /// Helper method to retrieve a bool from PlayerPrefs (stored as an int)
        /// </summary>
        public static bool GetBool(string key, bool defaultValue = false)
        {
            // Use HasKey to check if the bool has been stored (as int defaults to 0 which is ambiguous with a stored False)
            if (PlayerPrefs.HasKey(key))
            {
                int value = PlayerPrefs.GetInt(key);

                // As in C, assume zero is false and any non-zero value (including its intended 1) is true
                if (value != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // No existing player pref value, so return defaultValue instead
                return defaultValue;
            }
        }

        /// <summary>
        /// Helper method to store an enum value in PlayerPrefs (stored using the string name of the enum)
        /// </summary>
        public static void SetEnum(string key, Enum value)
        {
            // Convert the enum value to its string name (as opposed to integer index) and store it in a string PlayerPref
            PlayerPrefs.SetString(key, value.ToString());
        }

        /// <summary>
        /// Generic helper method to retrieve an enum value from PlayerPrefs and parse it from its stored string into the
        /// specified generic type. This method should generally be preferred over the non-generic equivalent
        /// </summary>
        public static T GetEnum<T>(string key, T defaultValue = default(T)) where T : struct
        {
            // Fetch the string value from PlayerPrefs
            string stringValue = PlayerPrefs.GetString(key);

            if (!string.IsNullOrEmpty(stringValue))
            {
                // Existing value, so parse it using the supplied generic type and cast before returning it
                return (T)Enum.Parse(typeof(T), stringValue);
            }
            else
            {
                // No player pref for this, just return default. If no default is supplied this will be the enum's default
                return defaultValue;
            }
        }

        /// <summary>
        /// Non-generic helper method to retrieve an enum value from PlayerPrefs (stored as a string). Default value must be
        /// passed, passing null will mean you need to do a null check where you call this method. Generally try to use the
        /// generic version of this method instead: GetEnum<T>
        /// </summary>
        public static object GetEnum(string key, Type enumType, object defaultValue)
        {
            // Fetch the string value from PlayerPrefs
            string value = PlayerPrefs.GetString(key);

            if (!string.IsNullOrEmpty(value))
            {
                // Existing value, parse it using the supplied type, then return the result as an object
                return Enum.Parse(enumType, value);
            }
            else
            {
                // No player pref for this key, so just return supplied default. It's required to supply a default value,
                // you can just pass null, but you would then need to do a null check where you call non-generic GetEnum().
                // Consider using GetEnum<T>() which doesn't require a default to be passed (supplying default(T) instead)
                return defaultValue;
            }
        }

        /// <summary>
        /// Helper method to store a DateTime (complete with its timezone) in PlayerPrefs as a string
        /// </summary>
        public static void SetDateTime(string key, DateTime value)
        {
            // Convert to an ISO 8601 compliant string ("o"), so that it's fully qualified, then store in PlayerPrefs
            PlayerPrefs.SetString(key, value.ToString("o", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Helper method to retrieve a DateTime from PlayerPrefs (stored as a string) and return a DateTime complete with
        /// timezone (works with UTC and local DateTimes)
        /// </summary>
        public static DateTime GetDateTime(string key, DateTime defaultValue = new DateTime())
        {
            // Fetch the string value from PlayerPrefs
            string stringValue = PlayerPrefs.GetString(key);

            if (!string.IsNullOrEmpty(stringValue))
            {
                // Make sure to parse it using Roundtrip Kind otherwise a local time would come out as UTC
                return DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            else
            {
                // No existing player pref value, so return defaultValue instead
                return defaultValue;
            }
        }

        /// <summary>
        /// Helper method to store a TimeSpan in PlayerPrefs as a string
        /// </summary>
        public static void SetTimeSpan(string key, TimeSpan value)
        {
            // Use the TimeSpan's ToString() method to encode it as a string which is then stored in PlayerPrefs
            PlayerPrefs.SetString(key, value.ToString());
        }

        /// <summary>
        /// Helper method to retrieve a TimeSpan from PlayerPrefs (stored as a string)
        /// </summary>
        public static TimeSpan GetTimeSpan(string key, TimeSpan defaultValue = new TimeSpan())
        {
            // Fetch the string value from PlayerPrefs
            string stringValue = PlayerPrefs.GetString(key);

            if (!string.IsNullOrEmpty(stringValue))
            {
                // Parse the string and return the TimeSpan
                return TimeSpan.Parse(stringValue);
            }
            else
            {
                // No existing player pref value, so return defaultValue instead
                return defaultValue;
            }
        }
    }

}
