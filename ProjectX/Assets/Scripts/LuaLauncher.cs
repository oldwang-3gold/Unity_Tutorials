using System;
using System.IO;
using System.Collections;
using UnityEngine;
using XLua;

public class LuaLauncher : MonoBehaviour
{
    public string scriptBootstrap;
    private static LuaLauncher s_Instance;
    public static LuaLauncher Instance { get { return s_Instance; } }

    public LuaEnv env { get; private set; }

    private LuaTable m_Ltentry;

    private Action m_ActStart;
    private Action m_ActUpdate;
    private Action<string> m_ActHotfix;


    private byte[] loader(ref string luapath)
    {
        return ResourceMgr.Instance.LoadLua(luapath);
    }

    void Bootstrap()
    {
        env = new LuaEnv();

        env.L.GetGlobal("_G");
        env.L.Pop(1);
        env.AddLoader(loader);

        byte[] bt = loader(ref scriptBootstrap);
        object[] objs = env.DoString(bt, scriptBootstrap);
        if (objs == null || objs.Length == 0)
        {
            Debug.LogError($"lua launcher load bootstrap fail. path: {scriptBootstrap}");
            return;
        }

        m_Ltentry = objs[0] as LuaTable;
        if (m_Ltentry == null)
        {
            Debug.LogError($"get ltentry fail. objs.length = {objs.Length}");
            return;
        }

        m_Ltentry.Set("_mono", this);
        m_Ltentry.Get("start", out m_ActStart);
        m_Ltentry.Get("update", out m_ActUpdate);
        m_Ltentry.Get("hotfix", out m_ActHotfix);
    }

    private void Awake()
    {
        s_Instance = this;

        Debug.Log("LuaLauncher Awake");
        Application.runInBackground = true;

        Bootstrap();
        m_ActStart?.Invoke();
    }

    private void Update()
    {
        if (env == null)
        {
            return;
        }
        m_ActUpdate?.Invoke();
    }

    public void LuaHotfix(string luapath)
    {
        m_ActHotfix?.Invoke(luapath);
    }

    public void Dispose()
    {
        env.Tick();
        env.FullGc();
        env.Dispose(true);
        env = null;

        m_Ltentry = null;
        m_ActStart = null;
        m_ActUpdate = null;

        GC.Collect();

        Debug.Log("LuaLauncher disposed...");
    }

}
