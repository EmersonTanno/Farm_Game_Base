using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : MonoBehaviour
{
    public static WarpController Instance;

    private bool isWarping;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ExecuteWarp(WarpTile warp)
    {
        if (isWarping) return;
        isWarping = true;

        SceneController.Instance.LoadScene(
            warp.scene,
            new Vector2(warp.x, warp.y)
        );
    }

    public void EndWarp()
    {
        isWarping = false;
    }
}