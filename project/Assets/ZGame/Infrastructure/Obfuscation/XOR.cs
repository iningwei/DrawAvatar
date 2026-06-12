using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ZGame.Obfuscation
{
    public class XOR
    {
        public static string EncryptStr(string target, string secretKey)
        {
            char[] data = target.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new string(data);
        }

        public static string DecryptStr(string target, string secretKey)
        {
            char[] data = target.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new string(data);
        }


        public static void EncryptFile(string filePath, string key)
        {
            if (key.Trim() == "")
            {
                Debug.LogError("error,key can not be empty!");
                return;
            }
            var data_bytes = File.ReadAllBytes(filePath);
            var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < data_bytes.Length; i++)
            {
                data_bytes[i] = (byte)(data_bytes[i] ^ key_bytes[i % key_bytes.Length]);
            }
            File.WriteAllBytes(filePath, data_bytes);
        }

        public static byte[] DecryptFile(string filePath, string key)
        {
            if (key.Trim() == "")
            {
                Debug.LogError("error,key can not be empty!");
                return null;
            }
            var data_bytes = File.ReadAllBytes(filePath);
            var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < data_bytes.Length; i++)
            {
                data_bytes[i] ^= key_bytes[i % key_bytes.Length];
            }
            return data_bytes;
        }

        public static byte[] EncryptBytes(byte[] data_bytes, string key)
        {
            if (key.Trim() == "")
            {
                Debug.LogError("error,key can not be empty!");
                return null;
            }

            var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0; i < data_bytes.Length; i++)
            {
                data_bytes[i] = (byte)(data_bytes[i] ^ key_bytes[i % key_bytes.Length]);
            }
            return data_bytes;
        }

        public static byte[] DecryptBytes(byte[] data_bytes, string key)
        {
            if (key.Trim() == "")
            {
                Debug.LogError("error,key can not be empty!");
                return null;
            }

            var key_bytes = System.Text.Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < data_bytes.Length; i++)
            {
                data_bytes[i] ^= key_bytes[i % key_bytes.Length];
            }
            return data_bytes;
        }


    }
}