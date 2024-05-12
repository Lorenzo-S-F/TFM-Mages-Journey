
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            return _instance;
        }
    }
}
