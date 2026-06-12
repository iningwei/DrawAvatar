using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ZGame.Obfuscation
{
    public class MD5
    {
        public static string Get(byte[] data)
        {
            var encoded = new MD5CryptoServiceProvider().ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encoded.Length; i++)
                sb.Append(encoded[i].ToString("x2"));

            return sb.ToString();
        }

    }
}