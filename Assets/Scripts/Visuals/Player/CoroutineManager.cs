using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager m_Instance;
    public static CoroutineManager Instance 
    {
        get 
        { 
            if (m_Instance == null)
            {
                GameObject go = new GameObject("Coroutine Manager");
                m_Instance = go.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(go);
            }

            return m_Instance; 
        }
    }
}
