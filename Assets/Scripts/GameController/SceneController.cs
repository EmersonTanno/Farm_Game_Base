using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public Animator animator;

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
        if (hasPendingTeleport) return;

        targetPlayerPosition = spawnPosition;
        hasPendingTeleport = true;

        StartCoroutine(LoadLevelAsync(warp.scene));
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
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

        animator.SetTrigger("End");

        hasPendingTeleport = false;
        WarpController.Instance.EndWarp();
    }
}
