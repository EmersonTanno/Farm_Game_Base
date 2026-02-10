using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void KillSession()
    {
        Destroy(gameObject);
    }
}