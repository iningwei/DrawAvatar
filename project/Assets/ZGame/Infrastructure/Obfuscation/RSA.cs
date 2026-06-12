using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class RSA
{
    /// <summary>
    /// 生成RSA私钥 公钥
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="publicKey"></param>
    public static void RSAGenerateKey(ref string privateKey, ref string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        privateKey = rsa.ToXmlString(true);
        publicKey = rsa.ToXmlString(false);
    }

    /// <summary>
    /// 用RSA公钥 加密 (data过长会报错：System.Security.Cryptography.CryptographicException: The data to be encrypted exceeds the maximum for this modulus of 245 bytes.)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKey">xml格式</param>
    /// <returns></returns>
    //public static byte[] RSAEncrypt(byte[] data, string publicKey)
    //{
    //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
    //    rsa.FromXmlString(publicKey);
    //    byte[] encryptData = rsa.Encrypt(data, false);
    //    return encryptData;
    //}



    //在RSA加密中，单次加密的数据长度受限于密钥的位数。通常，对于2048位的RSA密钥，单次加密的最大数据长度为245字节。这是因为RSA加密算法会在加密前给数据添加填充（padding），从而减少了实际可用的字节数。
    //将数据分割成245字节以下的小块进行逐段加密。然后，将每段加密结果拼接起来。这样可以确保每段数据都在可加密的范围内。
    /// <summary>
    /// 用RSA公钥 加密
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKey">xml格式</param>
    /// <returns></returns>
    public static byte[] RSAEncrypt(byte[] data, string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKey);

        int blockSize = (rsa.KeySize / 8) - 11; // 每块的大小（考虑填充）//rsa.KeySize:2048, blockSize:245 
        List<byte> encryptedData = new List<byte>();

        for (int i = 0; i < data.Length; i += blockSize)
        {
            byte[] block = data.Skip(i).Take(blockSize).ToArray();
            byte[] encryptedBlock = rsa.Encrypt(block, false);
            encryptedData.AddRange(encryptedBlock);
        }

        return encryptedData.ToArray();
    }


    /// <summary>
    /// 用RSA私钥 解密
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static byte[] RSADecrypt(byte[] data, string privateKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(privateKey);
        byte[] decryptData = rsa.Decrypt(data, false);
        return decryptData;
    }


}
