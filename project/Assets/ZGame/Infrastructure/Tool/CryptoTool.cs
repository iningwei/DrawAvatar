using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZGame
{
    public class CryptoTool
    {

        /// <summary>
        /// Des加密字符串，结果以16进制表示
        /// （不会像base64编码那样产生特殊字符= + /等）
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesEncryptStrWithHex(string content, string key)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            if (key.Length != 8)
            {
                Debug.LogError("DesEncryptStr error des need key has 8 char");
                return "";
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] byteContent = Encoding.UTF8.GetBytes(content);

            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);


            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(byteContent, 0, byteContent.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            return ret.ToString();
        }

        public static string DesDecryptStrFromHex(string content, string skey)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            if (skey.Length != 8)
            {
                Debug.LogError("DesDecryptStr error des need key has 8 char");
                return "";
            }
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] byteContent = new byte[content.Length / 2];
                for (int x = 0; x < content.Length / 2; x++)
                {
                    int ret = Convert.ToInt32(content.Substring(x * 2, 2), 16);
                    byteContent[x] = (byte)ret;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(skey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(skey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(),
                  CryptoStreamMode.Write);
                cs.Write(byteContent, 0, byteContent.Length);
                cs.FlushFinalBlock();
                string result = Encoding.Default.GetString(ms.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("DesDecryptStrFromHex error, content:" + content + ", ex:" + ex.ToString());
                return null;
            }

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key">秘钥必须为8位</param>
        /// <returns>返回的串是经过base64处理的</returns>
        public static string DesEncryptStrWithBase64(string content, string key)
        {
            if (key.Length != 8)
            {
                Debug.LogError("DesEncryptStr error des need key has 8 char");
                return "";
            }




            //        byte[] rgbKey = Encoding.UTF8.GetBytes(key);
            //        byte[] rgbIV = Encoding.UTF8.GetBytes(key);
            //        byte[] inputByteArry = Encoding.UTF8.GetBytes(content);

            //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //        des.Padding = PaddingMode.PKCS7;
            //        des.Mode = CipherMode.ECB;

            //        MemoryStream ms = new MemoryStream();
            //        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(rgbKey, rgbIV)
            //, CryptoStreamMode.Write);
            //        cs.Write(inputByteArry, 0, inputByteArry.Length);
            //        cs.FlushFinalBlock();
            //        return Convert.ToBase64String(ms.ToArray());



            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = Encoding.UTF8.GetBytes(key);
            byte[] bStr = Encoding.UTF8.GetBytes(content);

            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            desc.Padding = PaddingMode.PKCS7;//补位
            desc.Mode = CipherMode.ECB;//CipherMode.CBC

            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, desc.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                {
                    cStream.Write(bStr, 0, bStr.Length);
                    cStream.FlushFinalBlock();
                    byte[] res = mStream.ToArray();
                    return Convert.ToBase64String(res);
                }
            }
        }
















    }
}