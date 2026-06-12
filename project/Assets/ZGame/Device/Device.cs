using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Device
{
    public class Device
    {
        public static string iosDeviceOSVersion()
        {
            string iosDeviceOSVersion = "";
#if UNITY_IOS
            iosDeviceOSVersion = UnityEngine.iOS.Device.systemVersion;
#endif
            return iosDeviceOSVersion;
        }
        public static int iosDeviceOSBigVersion()
        {
            int iosDeviceOSBigVersion = int.Parse(iosDeviceOSVersion().Split('.')[0]);
            return iosDeviceOSBigVersion;
        }
    }
}