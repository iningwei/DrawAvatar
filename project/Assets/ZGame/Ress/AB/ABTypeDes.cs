using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZGame.Ress.AB
{

    [AttributeUsage(AttributeTargets.Field)]
    public class ABTypeDes : Attribute
    {
        //打成ab后的前缀
        public string PreFix { get; private set; }
        public ABTypeDes(string preFix)
        {
            this.PreFix = preFix;
        }
    }


    public class ABTypeUtil
    {
        public static string GetPreFix(ABType? t)
        {
            if (t is ABType)
            {
                FieldInfo[] fields = t.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.Equals(t.ToString()))
                    {
                        object[] objs = field.GetCustomAttributes(typeof(ABTypeDes), true);
                        if (objs != null && objs.Length > 0)
                        {
                            return ((ABTypeDes)objs[0]).PreFix;
                        }
                        else
                        {
                            DebugExt.LogE(t.GetType().Name + "，have not add ABTypeDes");
                            return "";
                        }
                    }
                }

                DebugExt.LogE("no such field:" + t.GetType().Name);
            }

            return "";
        }
    }
}