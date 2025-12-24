using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraBinder : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var cam = FindObjectOfType<CinemachineVirtualCamera>();

        if (player != null && cam != null)
        {
            cam.Follow = player.transform;
            cam.LookAt = player.transform;
        }
    }
}