using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using Object = UnityEngine.Object;

public class ResourceMgr : SingletonMonoBehaviour<ResourceMgr>
{
    public static T LoadFromResources<T>(string resPath) where T : Object
    {
        Object o = Resources.Load<T>(resPath);
        return (T)o;
    }

    private static string processResPath(string respath)
    {
        return respath.Trim().Replace('.', '/').ToLower();
    }

    public byte[] LoadLua(string luamodule)
    {
        byte[] bt = null;
        if (bt == null)
        {
            luamodule = processResPath(luamodule);
            string luapath = $"Assets/Lua/{luamodule}.lua";
            if (File.Exists(luapath))
            {
                bt = File.ReadAllBytes(luapath);
            }
            else
            {
                Debug.LogError($"can not load lua from editor [ {luamodule} ]");
            }
            if (bt == null)
            {
                TextAsset ta = LoadFromResources<TextAsset>(luapath);
                if (ta != null)
                {
                    bt = ta.bytes;
                }
            }
        }
        return bt;
    }
}
