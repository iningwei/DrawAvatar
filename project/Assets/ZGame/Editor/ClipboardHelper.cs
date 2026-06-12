using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//come from
//http://answers.unity3d.com/questions/266244/how-can-i-add-copypaste-clipboard-support-to-my-ga.html

public class ClipboardHelper
{
    private static PropertyInfo m_systemCopyBufferProperty = null;
    private static PropertyInfo GetSystemCopyBufferProperty()
    {
        if (m_systemCopyBufferProperty == null)
        {

            Type T = typeof(GUIUtility);
            m_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer");// , BindingFlags.Static| BindingFlags.NonPublic//加上这个报错
            if (m_systemCopyBufferProperty == null)
                throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
        }
        return m_systemCopyBufferProperty;
    }
    public static string clipBoard
    {
        get
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            return (string)P.GetValue(null, null);
        }
        set
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            P.SetValue(null, value, null);
        }
    }
}

