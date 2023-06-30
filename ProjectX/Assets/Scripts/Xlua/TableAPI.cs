using UnityEngine;
using System.Collections;
#if ULUA
using LuaInterface;
#else
using XLua;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
using LuaState = XLua.LuaEnv;
#endif
using ILuaState = System.IntPtr;

public static class TableAPI
{
    public static void SetDict(this ILuaState self, string key, LuaCSFunction func)
    {
        self.PushString(key);
        self.PushCSharpFunction(func);
        self.RawSet(-3);
    }
}
