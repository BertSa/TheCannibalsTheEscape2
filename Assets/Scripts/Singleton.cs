using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    public static bool IsInitialized => Instance != null;

    protected virtual void Awake()
    {
        if (!IsInitialized)
            Instance = (T) this;
        else
            print("[Singleton] trying to instantiate a second instance of singleton class");
    }
}