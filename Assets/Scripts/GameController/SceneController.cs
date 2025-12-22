using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private Vector3 targetPlayerPosition;
    private bool hasPendingTeleport;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(WarpTile warp, Vector2 spawnPosition)
    {
        targetPlayerPosition = spawnPosition;
        hasPendingTeleport = true;

        SceneManager.LoadScene(warp.scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasPendingTeleport) return;

        Player_Controller player = FindObjectOfType<Player_Controller>();

        if (player != null)
        {
            player.transform.position = targetPlayerPosition + new Vector3(0.5f, 0.7f, 0);
            player.movePoint.position = player.transform.position;
        }

        hasPendingTeleport = false;
        WarpController.Instance.EndWarp();
    }
}
