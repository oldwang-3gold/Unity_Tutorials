using System;
using UnityEngine;

public class Singleton<T> : IDisposable where T : new()
{
    static Singleton()
    {

    }

    public virtual void Dispose()
    {
        Dispose(true);
    }

    protected virtual void DisposeGC() { }

    private void Dispose(Boolean disposing)
    {
        if (disposing)
            DisposeGC();
    }

    protected static T s_Object = default(T);
    public static T Instance
    {
        get
        {
            if (null == s_Object)
            {
                s_Object = new T();
                if (null == s_Object)
                {
                    Debug.LogError("Error Create Singleton !" + s_Object.GetType().ToString());
                }
            }
            return s_Object;
        }
        set { s_Object = value; }
    }
}

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T s_S;
    public static T Instance
    {
        get
        {
            if (null == s_S)
            {
                s_S = (T)GameObject.FindObjectOfType(typeof(T));
                if (null == s_S)
                {
                    GameObject instanceObject = new GameObject(typeof(T).Name);
                    s_S = instanceObject.AddComponent<T>();

                    GameObject parent = GameObject.Find("Singleton");
                    if (parent == null)
                    {
                        parent = new GameObject("Singleton");
                        GameObject.DontDestroyOnLoad(parent);
                    }
                    parent.transform.SetAsLastSibling();
                    instanceObject.transform.parent = parent.transform;
                }
            }
            return s_S;
        }
    }

    private void Awake()
    {
        if (s_S == null)
        {
            s_S = this as T;
        }

        DontDestroyOnLoad(gameObject);
        Init();
    }

    protected virtual void Init()
    {

    }

    public void DestroySelf()
    {
        Dispose();
        SingletonMonoBehaviour<T>.s_S = null;
        UnityEngine.Object.Destroy(gameObject);
    }

    public virtual void Dispose()
    {

    }
}
