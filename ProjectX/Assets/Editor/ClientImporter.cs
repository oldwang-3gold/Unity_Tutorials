using UnityEngine;
using UnityEditor;
using System.IO;

public class ClientImporter : AssetPostprocessor
{
    private const string m_LuaRoot = "Assets/Lua";

    private void OnPreprocessLua(string path)
    {
        if (!Application.isPlaying)
        {
            return;
        }
        int index = path.IndexOf(m_LuaRoot);
        if (index < 0)
        {
            return;
        }

        if (Path.GetExtension(path) != ".lua")
        {
            return;
        }

        string luapath = path.Substring(index + m_LuaRoot.Length + 1);
        luapath = luapath.Replace('\\', '/');
        luapath = luapath.Replace('/', '.');
        luapath = Path.GetFileNameWithoutExtension(luapath);

        LuaLauncher.Instance.LuaHotfix(luapath);
    }

    private void OnPreprocessAsset()
    {
        if (assetPath.Contains(m_LuaRoot))
        {
            OnPreprocessLua(assetPath);
        }
    }
}
