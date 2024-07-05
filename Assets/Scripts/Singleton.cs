using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    // Property to access the Singleton instance
    public static T Instance
    {
        get
        {
            // If there is no instance, find one
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
            }
            return _instance;
        }
    }
}
