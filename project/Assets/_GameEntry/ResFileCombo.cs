using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using ZGame;

/// <summary>
/// 文件集合下载相关工具类
/// </summary>
public class ResFileCombo
{
    MemoryStream stream = null;
    BinaryWriter writer = null;
    BinaryReader reader = null;

    //public ABFileComboTool()
    //{
    //    stream = new MemoryStream();
    //    writer = new BinaryWriter(stream);
    //}
    public ResFileCombo(byte[] data = null)
    {
        if (data != null)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
            tablist = new List<ResFileData>();
        }
        else
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    List<ResFileData> tablist;
    public List<ResFileData> ReadAllFile()
    {
        stream.Position = 0;
        try
        {
            while (stream.Position < stream.Length)
            {
                ResFileData tab = new ResFileData();
                tab.name = ReadString();
                tab.data = ReadABBytes();
                tablist.Add(tab);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(" fetal error: file format error:" + ex.Message);
            tablist = new List<ResFileData>();
        }
        return tablist;
    }
    public void Close()
    {
        if (writer != null) writer.Close();
        if (reader != null) reader.Close();

        stream.Close();
        writer = null;
        reader = null;
        stream = null;
    }

    public void WriteByte(int v)
    {
        writer.Write((byte)v);
    }
    public void WriteBytes(byte[] arr)
    {
        writer.Write(arr.Length);
        writer.Write(arr);
    }
    public void WriteInt(int v)
    {
        writer.Write((int)v);
    }

    public void WriteShort(ushort v)
    {
        writer.Write((ushort)v);
    }

    public void WriteString(string v)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(v);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    public int ReadByte()
    {
        return (int)reader.ReadByte();
    }
    /// <summary>
    /// 必须是正确的长度
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    public byte[] ReadABBytes()
    {
        int len = ReadInt();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return buffer;
    }
    public int ReadInt()
    {
        return (int)reader.ReadInt32();
    }

    public ushort ReadShort()
    {
        return (ushort)reader.ReadInt16();
    }


    public string ReadString()
    {
        ushort len = ReadShort();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte[] ToBytes()
    {
        writer.Flush();
        return stream.ToArray();
    }

    public void Flush()
    {
        writer.Flush();
    }
}

public class ResFileData
{
    public string name;
    public byte[] data;
}