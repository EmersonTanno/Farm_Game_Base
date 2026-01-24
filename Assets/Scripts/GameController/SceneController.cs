using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Transitions")]
    [SerializeField] private List<TransitionType> loadingList;
    [SerializeField] private List<GameObject> loadingListCanvas;

    private Vector3 targetPlayerPosition;
    private bool hasPendingTeleport;

    //Flags
    private bool npcLoaded = false;

    private GameObject currentTransitionCanvas;
    public static event Action OnWarpStart;

    private void Awake()
    {
        if (loadingList.Count != loadingListCanvas.Count)
        {
            Debug.LogError("loadingList e loadingListCanvas têm tamanhos diferentes!");
        }

        foreach (var canvas in loadingListCanvas)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnEnable()
    {
        NPCController.OnNPCSet += NPCLoaded;
    }

    void OnDisable()
    {
        NPCController.OnNPCSet -= NPCLoaded;
    }

    public void LoadScene(WarpTile warp, Vector2 spawnPosition)
    {
        if (hasPendingTeleport) return;

        OnWarpStart?.Invoke();
        hasPendingTeleport = true;
        targetPlayerPosition = spawnPosition;
        Time_Controll.Instance.PauseTime();
        StartCoroutine(LoadLevelAsync(warp));
    }

    private IEnumerator LoadLevelAsync(WarpTile warp)
    {
        currentTransitionCanvas = GetTransitionCanvas(warp.transitionType);
        currentTransitionCanvas.SetActive(true);

        Animator anim = currentTransitionCanvas.GetComponent<Animator>();
        anim.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(warp.scene);
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
            Vector3 spawn = targetPlayerPosition + new Vector3(0.5f, 0.7f, 0);
            player.transform.position = spawn;
            player.movePoint.position = spawn;
        }

        if (currentTransitionCanvas != null)
        {
            Animator anim = currentTransitionCanvas.GetComponent<Animator>();
            anim.SetTrigger("End");
            StartCoroutine(DisableTransitionAfterAnim(currentTransitionCanvas, anim));
        }

        hasPendingTeleport = false;
        StartCoroutine(EndWarpNextFrame());
    }

    private IEnumerator EndWarpNextFrame()
    {
        while(!npcLoaded)
        {
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(1f);
        WarpController.Instance.EndWarp();
        yield return new WaitForSecondsRealtime(1f);
        Time_Controll.Instance.UnpauseTime();
        npcLoaded = false;
    }

    private void NPCLoaded()
    {
        Debug.Log("NPC Loaded");
        npcLoaded = true;
    }

    private IEnumerator DisableTransitionAfterAnim(GameObject canvas, Animator anim)
    {
        yield return new WaitForSecondsRealtime(
            anim.GetCurrentAnimatorStateInfo(0).length
        );

        canvas.SetActive(false);
        currentTransitionCanvas = null;
    }

    private GameObject GetTransitionCanvas(TransitionType type)
    {
        for (int i = 0; i < loadingList.Count; i++)
        {
            if (loadingList[i] == type)
                return loadingListCanvas[i];
        }

        Debug.LogError("TransitionType não encontrado: " + type);
        return loadingListCanvas[0];
    }
}
